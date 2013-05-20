#region Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
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
using System.Threading.Tasks;

using Autofac;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	internal class AuthenticationService : IAuthenticationService, INavigationHandler, IStartable
	{
		private readonly IAuthenticationServiceContext _authenticationServiceContext;
		private readonly IViewDiscoveryService _viewDiscoveryService;

		private readonly Dictionary<string, ViewDescriptor> _views = new Dictionary<string, ViewDescriptor>();
		private IWindowsPhoneNavigationService _navigationService;

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

		#region IStartable Members
		/// <summary>
		///     Perform once-off startup processing.
		/// </summary>
		public void Start()
		{
			var viewTypes = _viewDiscoveryService.GetViewTypes();

			foreach( var type in viewTypes )
			{
				var viewAttribute = type.GetCustomAttributes( typeof( ViewAttribute ), true ).Cast<ViewAttribute>().First();
				var viewName = viewAttribute.Name ?? type.Name;

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

		#region Internal methods
		internal void SetNavigationService( IWindowsPhoneNavigationService navigationService )
		{
			_navigationService = navigationService;
		}
		#endregion

		#region Miscellaneous
		private void CancelAuthenticationAndRollbackHistory()
		{
			if( _snapshotId != null )
			{
				var snapshotId = _snapshotId;

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

		private async Task StartAuthentication( string initialViewName, XParameters parameters, Action performWhenAuthenticated )
		{
			if( _authenticationServiceContext != null )
			{
				var authenticated = await _authenticationServiceContext.IsAuthenticated();

				if( !authenticated )
				{
					Action<object> guard = NavigationGuard;

					if( performWhenAuthenticated != null )
					{
						guard = async o =>
						{
							NavigationGuard( o );

							if( await _authenticationServiceContext.IsAuthenticated() )
							{
								performWhenAuthenticated();
							}
						};
					}

					_targetContext = null;
					_snapshotId = _navigationService.CreateSnapshot( guard );

					if( string.IsNullOrEmpty( initialViewName ) )
					{
						initialViewName = _authenticationServiceContext.AuthenticationViewName;
						parameters = _authenticationServiceContext.AuthenticationViewParameters;
					}

					_navigationService.Navigate( initialViewName, parameters );
				}
				else
				{
					if( performWhenAuthenticated != null )
					{
						performWhenAuthenticated();
					}
				}
			}
		}
		#endregion

		#region Implementation of INavigationHandler
		public async Task<bool> HandleNavigation( NavigationContext context )
		{
			ViewDescriptor descriptor;
			var result = false;

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
					var authenticated = await _authenticationServiceContext.IsAuthenticated();

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
			return StartAuthentication( null, null, null );
		}

		public Task StartAuthentication( Action performWhenAuthenticated )
		{
			return StartAuthentication( null, null, performWhenAuthenticated );
		}

		/// <summary>
		///     Completes the authentication.
		/// </summary>
		public void CompleteAuthentication()
		{
			if( _snapshotId != null )
			{
				var snapshotId = _snapshotId;
				var targetContext = _targetContext;

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

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		/// <param name="initialViewName">Name of the initial view in the authentication flow.</param>
		/// <param name="parameters">Parameters for the initial view.</param>
		public Task StartAuthentication( string initialViewName, XParameters parameters )
		{
			return StartAuthentication( initialViewName, parameters, null );
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