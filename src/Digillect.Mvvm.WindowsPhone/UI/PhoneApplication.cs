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

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Navigation;

using Autofac;

using Digillect.Mvvm.Services;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	///     Base class for Windows Phone applications.
	/// </summary>
	public abstract class PhoneApplication : Application
	{
		private bool _phoneApplicationInitialized;
		private PhoneApplicationService _phoneApplicationService;

		#region Constructors/Disposer
		/// <summary>
		///     Initializes a new instance of the <see cref="PhoneApplication" /> class.
		/// </summary>
		[SuppressMessage( "Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors" )]
		protected PhoneApplication()
		{
			InitializePhoneApplication();
			InitializeApplicationService();
			InitializeIoC();
		}
		#endregion

		#region Phone application initialization
		// Do not add any additional code to this method

		#region Event handlers
		private void CompleteInitializePhoneApplication( object sender, NavigationEventArgs e )
		{
			RootVisual = RootFrame;

			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}
		#endregion

		private void InitializePhoneApplication()
		{
			if( _phoneApplicationInitialized )
			{
				return;
			}

			// Create the frame but don't set it as RootVisual yet; this allows the splash
			// screen to remain active until the application is ready to render.
			RootFrame = CreateRootFrame();
			RootFrame.Navigated += CompleteInitializePhoneApplication;

			// Handle navigation failures
			RootFrame.NavigationFailed += RootFrame_NavigationFailed;

			// Ensure we don't initialize again
			_phoneApplicationInitialized = true;
		}

		/// <summary>
		///     Creates application root frame. By default creates instance of
		///     <see
		///         cref="Microsoft.Phone.Controls.PhoneApplicationFrame" />
		///     , override
		///     to create instance of other type.
		/// </summary>
		/// <returns>application frame.</returns>
		protected virtual PhoneApplicationFrame CreateRootFrame()
		{
			return new PhoneApplicationFrame();
		}
		#endregion

		#region Application Lifetime
		private void InitializeApplicationService()
		{
			_phoneApplicationService = CreateApplicationService();

			if( _phoneApplicationService != null )
			{
				_phoneApplicationService.Launching += ( o, e ) => HandleApplicationLaunching( e );
				_phoneApplicationService.Activated += ( o, e ) => HandleApplicationActivated( e );
				_phoneApplicationService.Deactivated += ( o, e ) => HandleApplicationDeactivated( e );
				_phoneApplicationService.Closing += ( o, e ) => HandleApplicationClosing( e );

				ApplicationLifetimeObjects.Add( _phoneApplicationService );
			}
		}

		/// <summary>
		///     Creates the instance of application service.
		/// </summary>
		/// <returns>
		///     New instance of phone application service or <c>null</c> to omit application service registration.
		/// </returns>
		protected virtual PhoneApplicationService CreateApplicationService()
		{
			return new PhoneApplicationService();
		}

		/// <summary>
		///     Handles the application launching.
		/// </summary>
		/// <param name="e">
		///     The <see cref="LaunchingEventArgs" /> instance containing the event data.
		/// </param>
		protected virtual void HandleApplicationLaunching( LaunchingEventArgs e )
		{
		}

		/// <summary>
		///     Handles the application activated.
		/// </summary>
		/// <param name="e">
		///     The <see cref="ActivatedEventArgs" /> instance containing the event data.
		/// </param>
		protected virtual void HandleApplicationActivated( ActivatedEventArgs e )
		{
		}

		/// <summary>
		///     Handles the application deactivated.
		/// </summary>
		/// <param name="e">
		///     The <see cref="DeactivatedEventArgs" /> instance containing the event data.
		/// </param>
		protected virtual void HandleApplicationDeactivated( DeactivatedEventArgs e )
		{
		}

		/// <summary>
		///     Handles the application closing.
		/// </summary>
		/// <param name="e">
		///     The <see cref="ClosingEventArgs" /> instance containing the event data.
		/// </param>
		protected virtual void HandleApplicationClosing( ClosingEventArgs e )
		{
		}
		#endregion

		#region Navigation

		#region Event handlers
		private void RootFrame_NavigationFailed( object sender, NavigationFailedEventArgs e )
		{
			HandleNavigationFailed( e );
		}
		#endregion

		/// <summary>
		///     Executes when navigation has been failed. Override to provide your own handling.
		/// </summary>
		/// <param name="e">
		///     The <see cref="System.Windows.Navigation.NavigationFailedEventArgs" /> instance containing the event data.
		/// </param>
		protected virtual void HandleNavigationFailed( NavigationFailedEventArgs e )
		{
		}
		#endregion

		#region IoC Support
		private void InitializeIoC()
		{
			ContainerBuilder builder = new ContainerBuilder();

			RegisterServices( builder );

			Scope = builder.Build();
		}

		/// <summary>
		///     Called to register services in IoC container.
		/// </summary>
		/// <param name="builder">The builder.</param>
		protected virtual void RegisterServices( ContainerBuilder builder )
		{
			builder.RegisterModule<WindowsPhoneModule>();
		}
		#endregion

		#region Public Properties
		/// <summary>
		///     Gets the application root frame.
		/// </summary>
		public PhoneApplicationFrame RootFrame { get; private set; }

		/// <summary>
		///     Gets the IoC container.
		/// </summary>
		public ILifetimeScope Scope { get; private set; }

		/// <summary>
		///     Gets the application service.
		/// </summary>
		/// <value>
		///     The application service.
		/// </value>
		public PhoneApplicationService ApplicationService
		{
			get { return _phoneApplicationService; }
		}
		#endregion
	}
}