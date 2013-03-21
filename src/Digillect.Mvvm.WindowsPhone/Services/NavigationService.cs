using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

using Autofac;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Windows phone implementation of <see cref="Digillect.Mvvm.Services.INavigationService" />
	/// </summary>
	public sealed class NavigationService : INavigationService, IAuthenticationService, IStartable
	{
		const string AuthenticationSourceStateKey = "___AUTH_SOURCE___";
		const string AuthenticationTargetStateKey = "___AUTH_TARGET___";

		readonly IAuthenticationServiceContext _authenticationServiceContext;
		readonly INavigationServiceContext _navigationServiceContext;

		readonly Dictionary<string, ViewDescriptor> _views = new Dictionary<string, ViewDescriptor>( StringComparer.InvariantCultureIgnoreCase );

		bool _authenticationInProgress;
		bool _initialized;
		bool _removeLastJournalEntry;

		#region Constructors/Disposer
		/// <summary>
		///     Initializes a new instance of the <see cref="NavigationService" /> class.
		/// </summary>
		/// <param name="navigationServiceContext">The navigation service context.</param>
		public NavigationService( INavigationServiceContext navigationServiceContext )
			: this( navigationServiceContext, null )
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="NavigationService" /> class.
		/// </summary>
		/// <param name="navigationServiceContext">The navigation service context.</param>
		/// <param name="authenticationServiceContext">Application extention that helps with authentication.</param>
		public NavigationService( INavigationServiceContext navigationServiceContext, IAuthenticationServiceContext authenticationServiceContext )
		{
			_navigationServiceContext = navigationServiceContext;
			_authenticationServiceContext = authenticationServiceContext;
		}
		#endregion

		#region IAuthenticationService Members
		/// <summary>
		///     Gets a value indicating whether authentication is in progress.
		/// </summary>
		/// <value>
		///     <c>true</c> if authentication is in progress; otherwise, <c>false</c>.
		/// </value>
		public bool AuthenticationInProgress
		{
			get { return _authenticationInProgress; }
		}

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		public void StartAuthentication()
		{
			StartAuthentication( null, null );
		}

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		/// <param name="initialViewName">Name of the initial view in the authentication flow.</param>
		/// <param name="parameters">Parameters for the initial view.</param>
		public async void StartAuthentication( string initialViewName, Parameters parameters )
		{
			if( _authenticationServiceContext != null )
			{
				Tuple<Uri, ViewDescriptor> result = await BeginAuthentication( null, initialViewName, parameters );

				if( result.Item1 != null )
				{
					_navigationServiceContext.Navigate( result.Item1 );
				}
			}
		}

		/// <summary>
		///     Cancels the authentication.
		/// </summary>
		public void CancelAuthentication()
		{
			CancelAuthenticationAndRollbackHistory();
		}

		/// <summary>
		///     Completes the authentication.
		/// </summary>
		public void CompleteAuthentication()
		{
			var app = (PhoneApplication) Application.Current;
			var sourceUri = app.ApplicationService.State[AuthenticationSourceStateKey] as Uri;
			var targetUri = app.ApplicationService.State[AuthenticationTargetStateKey] as Uri;

			app.ApplicationService.State[AuthenticationSourceStateKey] = null;
			app.ApplicationService.State[AuthenticationTargetStateKey] = null;

			_authenticationInProgress = false;

			// Rewind journal

			int numberOfEntriesToRemove = 0;
			bool sourceUriIsFoundInBackStack = false;

			foreach( JournalEntry entry in app.RootFrame.BackStack )
			{
				if( entry.Source == sourceUri )
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
					app.RootFrame.RemoveBackEntry();
				}
			}

			if( targetUri != null )
			{
				_removeLastJournalEntry = true;
				_navigationServiceContext.Navigate( targetUri );
			}
			else
			{
				_navigationServiceContext.GoBack();
			}
		}
		#endregion

		#region INavigationService Members
		/// <summary>
		///     Navigates to the specified view.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		public void Navigate( string viewName )
		{
			Contract.Ensures( viewName != null );

			Navigate( viewName, null );
		}

		/// <summary>
		///     Navigates to the specified view with parameters.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		/// <param name="parameters">The parameters.</param>
		/// <exception cref="System.ArgumentNullException">viewName</exception>
		/// <exception cref="System.ArgumentException">viewName</exception>
		[CLSCompliant( false )]
		public async void Navigate( string viewName, Parameters parameters )
		{
			if( viewName == null )
			{
				throw new ArgumentNullException( "viewName" );
			}

			Contract.EndContractBlock();

			ViewDescriptor descriptor;

			if( !_views.TryGetValue( viewName, out descriptor ) )
			{
				throw new ArgumentException( String.Format( "View with name '{0}' is not registered.", viewName ), "viewName" );
			}

			var uri = new Uri( string.Format( "/{0}{1}", descriptor.Path, parameters != null ? "?" + string.Join( "&", parameters.Select( p => p.Key + "=" + ParametersSerializer.EncodeValue( p.Value ) ) ) : string.Empty ), UriKind.Relative );

			if( descriptor.AuthenticationRequired && _authenticationServiceContext != null )
			{
				Tuple<Uri, ViewDescriptor> result = await BeginAuthentication( uri, null, null );

				uri = result.Item1;

				if( result.Item2 != null )
				{
					descriptor = result.Item2;
				}
			}

			if( _authenticationInProgress && !descriptor.PartOfAuthentication )
			{
				CancelAuthenticationAndRollbackHistory();
			}
			else
			{
				_navigationServiceContext.Navigate( uri );
			}
		}

		/// <summary>
		///     Navigated to the previous view, if any.
		/// </summary>
		public void GoBack()
		{
			_navigationServiceContext.GoBack();
		}

		/// <summary>
		///     Navigates back until encounters view named <paramref name="viewName" />.
		/// </summary>
		/// <param name="viewName">Name of the view.</param>
		/// <exception cref="System.ArgumentNullException">viewName</exception>
		public void GoBack( string viewName )
		{
			if( viewName == null )
			{
				throw new ArgumentNullException( "viewName" );
			}

			_navigationServiceContext.GoBack( viewName );
		}
		#endregion

		#region IStartable Members
		/// <summary>
		///     Perform once-off startup processing.
		/// </summary>
		public void Start()
		{
			Assembly assemblyToScan = _navigationServiceContext.GetMainAssemblyContainingViews();
			string rootNamespace = _navigationServiceContext.GetRootNamespace();

			foreach( Type type in assemblyToScan.GetTypes().Where( t => !t.IsAbstract && t.GetCustomAttributes( typeof( ViewAttribute ), true ).Any() ) )
			{
				string typeName = type.FullName.StartsWith( rootNamespace ) ? type.FullName.Substring( rootNamespace.Length + 1 ) : type.FullName;
				ViewAttribute viewAttribute = type.GetCustomAttributes( typeof( ViewAttribute ), true ).Cast<ViewAttribute>().First();
				string viewName = viewAttribute.Name ?? type.Name;

				var descriptor = new ViewDescriptor
									{
										Name = viewName,
										Path = viewAttribute.Path ?? typeName.Replace( '.', '/' ) + ".xaml",
										AuthenticationRequired = viewAttribute.AuthenticationRequired,
										PartOfAuthentication = viewAttribute.PartOfAuthentication
									};

				_views.Add( viewName, descriptor );
			}

			var app = (PhoneApplication) Application.Current;

			app.RootFrame.Navigated += RootFrame_Navigated;
			app.RootFrame.Navigating += RootFrame_Navigating;
			app.RootFrame.NavigationFailed += RootFrame_NavigationFailed;
		}
		#endregion

		#region Event handlers
		void RootFrame_Navigated( object sender, NavigationEventArgs e )
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
				if( _authenticationInProgress )
				{
					var sourceUri = app.ApplicationService.State[AuthenticationSourceStateKey] as Uri;

					if( e.Uri == sourceUri )
					{
						app.ApplicationService.State[AuthenticationSourceStateKey] = null;
						app.ApplicationService.State[AuthenticationTargetStateKey] = null;

						_authenticationInProgress = false;
					}
				}
			}
		}

		void RootFrame_Navigating( object sender, NavigatingCancelEventArgs e )
		{
		}

		void RootFrame_NavigationFailed( object sender, NavigationFailedEventArgs navigationFailedEventArgs )
		{
		}
		#endregion

		void CompleteInitialization( NavigationEventArgs e )
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

		async Task<Tuple<Uri, ViewDescriptor>> BeginAuthentication( Uri targetUri, string initialViewName, Parameters parameters )
		{
			var app = (PhoneApplication) Application.Current;
			bool authenticated = await _authenticationServiceContext.IsAuthenticated();
			ViewDescriptor descriptor = null;

			if( !authenticated && !_authenticationInProgress )
			{
				app.ApplicationService.State[AuthenticationSourceStateKey] = app.RootFrame.CurrentSource;
				app.ApplicationService.State[AuthenticationTargetStateKey] = targetUri;

				if( string.IsNullOrEmpty( initialViewName ) )
				{
					initialViewName = _authenticationServiceContext.AuthenticationViewName;
					parameters = _authenticationServiceContext.AuthenticationViewParameters;
				}

				if( !_views.TryGetValue( initialViewName, out descriptor ) )
				{
					throw new InvalidOperationException( string.Format( "View with name '{0}' is not registered.", initialViewName ) );
				}

				targetUri = new Uri( string.Format( "/{0}{1}", descriptor.Path, parameters != null ? "?" + string.Join( "&", parameters.Select( p => p.Key + "=" + ParametersSerializer.EncodeValue( p.Value ) ) ) : string.Empty ), UriKind.Relative );

				_authenticationInProgress = true;
			}

			return Tuple.Create( targetUri, descriptor );
		}

		void CancelAuthenticationAndRollbackHistory()
		{
			var app = (PhoneApplication) Application.Current;
			var sourceUri = app.ApplicationService.State[AuthenticationSourceStateKey] as Uri;

			app.ApplicationService.State[AuthenticationSourceStateKey] = null;
			app.ApplicationService.State[AuthenticationTargetStateKey] = null;

			// Rewind journal

			int numberOfEntriesToRemove = 0;
			bool sourceUriIsFoundInBackStack = false;

			foreach( JournalEntry entry in app.RootFrame.BackStack )
			{
				if( entry.Source == sourceUri )
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
					app.RootFrame.RemoveBackEntry();
				}

				_navigationServiceContext.GoBack();
			}

			_authenticationInProgress = false;
		}

		#region Nested type: ViewDescriptor
		class ViewDescriptor
		{
			#region Public Properties
			public string Name { get; set; }
			public string Path { get; set; }
			public bool AuthenticationRequired { get; set; }
			public bool PartOfAuthentication { get; set; }
			#endregion
		}
		#endregion
	}
}