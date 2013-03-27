using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

using Autofac;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Windows phone implementation of <see cref="Digillect.Mvvm.Services.INavigationService" />
	/// </summary>
	internal sealed class NavigationService : IWindowsPhoneNavigationService, IStartable
	{
		private readonly INavigationHandler[] _navigationHandlers;
		private readonly List<NavigationSnapshot> _navigationSnapshots = new List<NavigationSnapshot>();
		private readonly IViewDiscoveryService _viewDiscoveryService;

		private readonly Dictionary<string, ViewDescriptor> _views = new Dictionary<string, ViewDescriptor>( StringComparer.InvariantCultureIgnoreCase );

		private bool _initialized;
		private bool _removeLastJournalEntry;

		#region Constructors/Disposer
		/// <summary>
		///     Initializes a new instance of the <see cref="NavigationService" /> class.
		/// </summary>
		/// <param name="viewDiscoveryService">The view discovery service.</param>
		/// <param name="navigationHandlers">The navigation handlers.</param>
		public NavigationService( IViewDiscoveryService viewDiscoveryService, IEnumerable<INavigationHandler> navigationHandlers )
		{
			_viewDiscoveryService = viewDiscoveryService;
			_navigationHandlers = navigationHandlers.ToArray();
		}
		#endregion

		#region IStartable Members
		/// <summary>
		///     Perform once-off startup processing.
		/// </summary>
		public void Start()
		{
			string rootNamespace = _viewDiscoveryService.GetRootNamespace();
			IEnumerable<Type> viewTypes = _viewDiscoveryService.GetViewTypes();

			foreach( Type type in viewTypes )
			{
				string typeName = type.FullName.StartsWith( rootNamespace ) ? type.FullName.Substring( rootNamespace.Length + 1 ) : type.FullName;
				ViewAttribute viewAttribute = type.GetCustomAttributes( typeof( ViewAttribute ), true ).Cast<ViewAttribute>().First();
				ViewPathAttribute viewPathAttribute = type.GetCustomAttributes( typeof( ViewPathAttribute ), false ).Cast<ViewPathAttribute>().FirstOrDefault();
				string viewName = viewAttribute.Name ?? type.Name;

				var descriptor = new ViewDescriptor
									{
										Name = viewName,
										Type = type,
										Path = viewPathAttribute == null ? typeName.Replace( '.', '/' ) + ".xaml" : viewPathAttribute.Path,
									};

				_views.Add( viewName, descriptor );
			}

			var app = (PhoneApplication) Application.Current;

			app.RootFrame.Navigated += RootFrame_Navigated;
		}
		#endregion

		#region IWindowsPhoneNavigationService Members
		/// <summary>
		///     Navigates to the specified view.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		public void Navigate( string viewName )
		{
			Navigate( viewName, null );
		}

		/// <summary>
		///     Navigates to the specified view with parameters.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		/// <param name="parameters">The parameters.</param>
		/// <exception cref="System.ArgumentNullException">viewName</exception>
		/// <exception cref="System.ArgumentException">viewName</exception>
		public async void Navigate( string viewName, Parameters parameters )
		{
			var context = new NavigationContext { ViewName = viewName, Parameters = parameters };

			foreach( INavigationHandler handler in _navigationHandlers )
			{
				if( await handler.HandleNavigation( context ) )
				{
					break;
				}
			}

			if( context.Cancel )
			{
				return;
			}

			ViewDescriptor descriptor;

			if( !_views.TryGetValue( context.ViewName, out descriptor ) )
			{
				throw new ArgumentException( String.Format( "View with name '{0}' is not registered.", context.ViewName ), "viewName" );
			}

			var uri = new Uri( string.Format( "/{0}{1}", descriptor.Path, context.Parameters != null ? "?" + string.Join( "&", context.Parameters.Select( p => p.Key + "=" + ParametersSerializer.EncodeValue( p.Value ) ) ) : string.Empty ), UriKind.Relative );

			if( context.DisplaceCurrentView )
			{
				_removeLastJournalEntry = true;
			}

			((PhoneApplication) Application.Current).RootFrame.Navigate( uri );
		}

		/// <summary>
		///     Navigated to the previous view, if any.
		/// </summary>
		public void GoBack()
		{
			((PhoneApplication) Application.Current).RootFrame.GoBack();
		}

		public object CreateSnapshot()
		{
			return CreateSnapshot( null, null );
		}

		public object CreateSnapshot( Action<object> guard )
		{
			return CreateSnapshot( guard, null );
		}

		public object CreateSnapshot( Action<object> guard, object tag )
		{
			var app = (PhoneApplication) Application.Current;
			var snapshot = new NavigationSnapshot { Uri = app.RootFrame.CurrentSource, Guard = guard, Tag = tag };

			_navigationSnapshots.Add( snapshot );

			return snapshot;
		}

		public bool RollbackSnapshot( object snapshotId )
		{
			return RollbackSnapshot( snapshotId, null, null );
		}

		public bool RollbackSnapshot( object snapshotId, string viewName, Parameters parameters )
		{
			var snapshot = snapshotId as NavigationSnapshot;

			if( snapshot == null )
			{
				throw new ArgumentException( "Invalid snapshot.", "snapshotId" );
			}

			// Rewind journal
			var app = (PhoneApplication) Application.Current;
			int numberOfEntriesToRemove = 0;
			bool sourceUriIsFoundInBackStack = false;

			foreach( JournalEntry entry in app.RootFrame.BackStack )
			{
				if( entry.Source == snapshot.Uri )
				{
					sourceUriIsFoundInBackStack = true;
					break;
				}

				numberOfEntriesToRemove++;
			}

			if( sourceUriIsFoundInBackStack )
			{
				while( numberOfEntriesToRemove-- > 0 )
				{
					JournalEntry entry = app.RootFrame.RemoveBackEntry();

					ProcessSnapshots( entry.Source );
				}

				if( viewName != null )
				{
					_removeLastJournalEntry = true;
					Navigate( viewName, parameters );
				}
				else
				{
					app.RootFrame.GoBack();
				}

				return true;
			}

			return false;
		}
		#endregion

		#region Event handlers
		private void RootFrame_Navigated( object sender, NavigationEventArgs e )
		{
			var app = (PhoneApplication) Application.Current;

			if( !_initialized )
			{
				CompleteInitialization( e );
			}

			if( e.NavigationMode == NavigationMode.New )
			{
				if( _removeLastJournalEntry )
				{
					app.RootFrame.RemoveBackEntry();
					_removeLastJournalEntry = false;
				}
			}
			else if( e.NavigationMode == NavigationMode.Back )
			{
				ProcessSnapshots( e.Uri );
			}
		}
		#endregion

		private void ProcessSnapshots( Uri uri )
		{
			if( _navigationSnapshots.Count > 0 )
			{
				List<NavigationSnapshot> snapshots = _navigationSnapshots.Where( snapshot => snapshot.Uri == uri ).ToList();

				foreach( NavigationSnapshot snapshot in snapshots )
				{
					if( snapshot.Guard != null )
					{
						snapshot.Guard( snapshot.Tag );
					}

					_navigationSnapshots.Remove( snapshot );
				}
			}
		}

		private void CompleteInitialization( NavigationEventArgs e )
		{
			var app = (PhoneApplication) Application.Current;

			_initialized = true;

#if WINDOWS_PHONE_7
			if( e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh )
#else
			if( e.NavigationMode == NavigationMode.Reset )
#endif
			{
				while( app.RootFrame.RemoveBackEntry() != null )
				{
				}
			}
		}

		#region Nested type: NavigationSnapshot
		private class NavigationSnapshot
		{
			#region Public Properties
			public Uri Uri { get; set; }
			public Action<object> Guard { get; set; }
			public object Tag { get; set; }
			#endregion
		}
		#endregion

		#region Nested type: ViewDescriptor
		private class ViewDescriptor
		{
			#region Public Properties
			public string Name { get; set; }
			public Type Type { get; set; }
			public string Path { get; set; }
			#endregion
		}
		#endregion
	}
}