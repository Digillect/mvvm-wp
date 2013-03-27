using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	internal class AuthenticationService : IAuthenticationService, INavigationHandler, IStartable
	{
		private readonly IAuthenticationServiceContext _authenticationServiceContext;
		private IWindowsPhoneNavigationService _navigationService;
		private readonly IViewDiscoveryService _viewDiscoveryService;

		private readonly Dictionary<string, ViewDescriptor> _views = new Dictionary<string, ViewDescriptor>();

		private object _snapshotId;
		private NavigationContext _targetContext;

		#region Constructors/Disposer
		public AuthenticationService( IViewDiscoveryService viewDiscoveryService )
			: this( viewDiscoveryService, null )
		{
		}

		public AuthenticationService( IViewDiscoveryService viewDiscoveryService, IAuthenticationServiceContext authenticationServiceContext )
		{
			_authenticationServiceContext = authenticationServiceContext;
			_viewDiscoveryService = viewDiscoveryService;
		}
		#endregion

		internal void SetNavigationService( IWindowsPhoneNavigationService navigationService )
		{
			_navigationService = navigationService;
		}

		#region Implementation of IAuthenticationService
		/// <summary>
		///     Gets a value indicating whether authentication is in progress.
		/// </summary>
		/// <value>
		///     <c>true</c> if authentication is in progress; otherwise, <c>false</c>.
		/// </value>
		public bool AuthenticationInProgress
		{
			get { return _snapshotId != null; }
		}

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		public Task StartAuthentication()
		{
			return StartAuthentication( null, null );
		}

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		/// <param name="initialViewName">Name of the initial view in the authentication flow.</param>
		/// <param name="parameters">Parameters for the initial view.</param>
		public async Task StartAuthentication( string initialViewName, Parameters parameters )
		{
			if( _authenticationServiceContext != null )
			{
				var authenticated = await _authenticationServiceContext.IsAuthenticated();

				if( !authenticated )
				{
					_targetContext = null;
					_snapshotId = _navigationService.CreateSnapshot( NavigationGuard );

					if( string.IsNullOrEmpty( initialViewName ) )
					{
						initialViewName = _authenticationServiceContext.AuthenticationViewName;
						parameters = _authenticationServiceContext.AuthenticationViewParameters;
					}

					_navigationService.Navigate( initialViewName, parameters );
				}
			}
		}

		/// <summary>
		///     Completes the authentication.
		/// </summary>
		public void CompleteAuthentication()
		{
			if( _snapshotId != null )
			{
				object snapshotId = _snapshotId;
				NavigationContext targetContext = _targetContext;

				_snapshotId = null;
				_targetContext = null;

				if( targetContext != null )
				{
					_navigationService.RollbackSnapshot( snapshotId, targetContext.ViewName, targetContext.Parameters );
				}
				else
				{
					_navigationService.RollbackSnapshot( snapshotId );
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
		#endregion

		#region IStartable Members
		/// <summary>
		///     Perform once-off startup processing.
		/// </summary>
		public void Start()
		{
			IEnumerable<Type> viewTypes = _viewDiscoveryService.GetViewTypes();

			foreach( Type type in viewTypes )
			{
				ViewAttribute viewAttribute = type.GetCustomAttributes( typeof( ViewAttribute ), true ).Cast<ViewAttribute>().First();
				string viewName = viewAttribute.Name ?? type.Name;

				var descriptor = new ViewDescriptor
									{
										Name = viewName,
										Type = type,
										RequiresAuthentication = type.GetCustomAttributes( typeof( ViewRequiresAuthenticationAttribute ), false ).Any(),
										PartOfAuthentication = type.GetCustomAttributes( typeof( ViewIsPartOfAuthenticationFlowAttribute ), false ).Any()
									};

				_views.Add( viewName, descriptor );
			}
		}
		#endregion

		private void CancelAuthenticationAndRollbackHistory()
		{
			if( _snapshotId != null )
			{
				object snapshotId = _snapshotId;

				_snapshotId = null;
				_targetContext = null;

				_navigationService.RollbackSnapshot( snapshotId );
			}
		}

		private void NavigationGuard( object tag )
		{
			_snapshotId = null;
			_targetContext = null;
		}

		#region Implementation of INavigationHandler
		public async Task<bool> HandleNavigation( NavigationContext context )
		{
			ViewDescriptor descriptor;
			bool result = false;

			if( _authenticationServiceContext == null || !_views.TryGetValue( context.ViewName, out descriptor ) )
			{
				return false;
			}

			if( AuthenticationInProgress )
			{
				if( !descriptor.PartOfAuthentication )
				{
					context.Cancel = true;
					CancelAuthenticationAndRollbackHistory();

					result = true;
				}
			}
			else
			{
				if( descriptor.RequiresAuthentication )
				{
					bool authenticated = await _authenticationServiceContext.IsAuthenticated();

					if( !authenticated )
					{
						_targetContext = new NavigationContext { ViewName = context.ViewName, Parameters = context.Parameters };
						_snapshotId = _navigationService.CreateSnapshot( NavigationGuard );

						context.ViewName = _authenticationServiceContext.AuthenticationViewName;
						context.Parameters = _authenticationServiceContext.AuthenticationViewParameters;

						result = true;
					}
				}
			}

			return result;
		}
		#endregion

		#region Nested type: ViewDescriptor
		private class ViewDescriptor
		{
			#region Public Properties
			public string Name { get; set; }
			public Type Type { get; set; }
			public bool RequiresAuthentication { get; set; }
			public bool PartOfAuthentication { get; set; }
			#endregion
		}
		#endregion
	}
}