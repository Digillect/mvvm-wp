#region Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
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
#if WINDOWS_PHONE_71
	public
#else
	internal
#endif
	sealed class NavigationService : IWindowsPhoneNavigationService, IStartable
	{
		private readonly INavigationHandler[] _navigationHandlers;
		private readonly List<NavigationSnapshot> _navigationSnapshots = new List<NavigationSnapshot>();
		private readonly IViewDiscoveryService _viewDiscoveryService;

		private readonly Dictionary<string, ViewDescriptor> _views = new Dictionary<string, ViewDescriptor>( StringComparer.InvariantCultureIgnoreCase );

		private bool _removeLastJournalEntry;
		private bool _navigationIsInProgress;

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
			var rootNamespace = _viewDiscoveryService.GetRootNamespace();
			var viewTypes = _viewDiscoveryService.GetViewTypes();

			foreach( var type in viewTypes )
			{
				var typeName = type.FullName.StartsWith( rootNamespace ) ? type.FullName.Substring( rootNamespace.Length + 1 ) : type.FullName;
				var viewAttribute = type.GetCustomAttributes( typeof( ViewAttribute ), true ).Cast<ViewAttribute>().First();
				var viewPathAttribute = type.GetCustomAttributes( typeof( ViewPathAttribute ), false ).Cast<ViewPathAttribute>().FirstOrDefault();
				var viewName = viewAttribute.Name ?? type.Name;

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
			app.RootFrame.NavigationFailed += RootFrame_NavigationFailed;
			app.RootFrame.NavigationStopped += RootFrame_NavigationStopped;
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
		///     Navigated to the previous view, if any.
		/// </summary>
		public void GoBack()
		{
			if( !_navigationIsInProgress )
			{
				_navigationIsInProgress = true;

				((PhoneApplication) Application.Current).RootFrame.GoBack();
			}
		}

		/// <summary>
		///     Navigates back to the specified view or first view if not found.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		public void GoBack( string viewName )
		{
			if( !_navigationIsInProgress )
			{
				_navigationIsInProgress = true;

				var backStacks = ((PhoneApplication) Application.Current).RootFrame.BackStack;
				var journalEntries = backStacks as JournalEntry[] ?? backStacks.ToArray();

				ViewDescriptor descriptor;

				if( !_views.TryGetValue( viewName, out descriptor ) )
				{
					throw new ViewNavigationException( String.Format( "View '{0}' is not registered.", viewName ) );
				}

				for( int i = 0; i < journalEntries.Count() - 1; i++ )
				{
					if( !journalEntries[i].Source.ToString().Contains( descriptor.Path ) )
					{
						((PhoneApplication) Application.Current).RootFrame.RemoveBackEntry();
					}
					else
					{
						break;
					}
				}

				_navigationIsInProgress = false;
				GoBack();
			}
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

		/// <summary>
		///     Navigates to the specified view with parameters.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		/// <param name="parameters">The parameters.</param>
		/// <exception cref="System.ArgumentNullException">viewName</exception>
		/// <exception cref="System.ArgumentException">viewName</exception>
		public async void Navigate( string viewName, XParameters parameters )
		{
			if( _navigationIsInProgress )
			{
				return;
			}

			var context = new NavigationContext { ViewName = viewName, Parameters = parameters };

			foreach( var handler in _navigationHandlers )
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
				throw new ViewNavigationException( String.Format( "View '{0}' is not registered.", context.ViewName ) );
			}

			var uri = new Uri( string.Format( "/{0}{1}", descriptor.Path, context.Parameters != null ? "?" + string.Join( "&", context.Parameters.Select( p => p.Key + "=" + ParametersSerializer.EncodeValue( p.Value ) ) ) : string.Empty ), UriKind.Relative );

			if( context.DisplaceCurrentView )
			{
				_removeLastJournalEntry = true;
			}

			((PhoneApplication) Application.Current).RootFrame.Navigate( uri );
		}

		public bool RollbackSnapshot( object snapshotId, string viewName, XParameters parameters )
		{
			var snapshot = snapshotId as NavigationSnapshot;

			if( snapshot == null )
			{
				throw new ArgumentException( "Snapshot with provided ID is not found.", "snapshotId" );
			}

			// Rewind journal
			var app = (PhoneApplication) Application.Current;
			var numberOfEntriesToRemove = 0;
			var sourceUriIsFoundInBackStack = false;

			foreach( var entry in app.RootFrame.BackStack )
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
					var entry = app.RootFrame.RemoveBackEntry();

					ProcessSnapshots( entry.Source );
				}

				if( viewName != null )
				{
					_removeLastJournalEntry = true;
					Navigate( viewName, parameters );
				}
				else
				{
					GoBack();
				}

				return true;
			}

			return false;
		}
		#endregion

		#region Miscellaneous
		private void RootFrame_Navigated( object sender, NavigationEventArgs e )
		{
			var app = (PhoneApplication) Application.Current;

			_navigationIsInProgress = false;

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

		private void RootFrame_NavigationFailed( object sender, NavigationFailedEventArgs e )
		{
			_navigationIsInProgress = false;
		}

		private void RootFrame_NavigationStopped( object sender, NavigationEventArgs e )
		{
			_navigationIsInProgress = false;
		}

		private void ProcessSnapshots( Uri uri )
		{
			if( _navigationSnapshots.Count > 0 )
			{
				var snapshots = _navigationSnapshots.Where( snapshot => snapshot.Uri == uri ).ToList();

				foreach( var snapshot in snapshots )
				{
					if( snapshot.Guard != null )
					{
						snapshot.Guard( snapshot.Tag );
					}

					_navigationSnapshots.Remove( snapshot );
				}
			}
		}
		#endregion

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