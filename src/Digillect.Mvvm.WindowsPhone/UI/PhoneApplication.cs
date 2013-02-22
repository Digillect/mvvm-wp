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

		#region Constructors/Disposer
		/// <summary>
		///     Initializes a new instance of the <see cref="PhoneApplication" /> class.
		/// </summary>
		[SuppressMessage( "Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors" )]
		protected PhoneApplication()
		{
			InitializeIoC();
			InitializeApplicationService();
			InitializePhoneApplication();
		}
		#endregion

		#region Phone application initialization
		// Do not add any additional code to this method
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

		private void CompleteInitializePhoneApplication( object sender, NavigationEventArgs e )
		{
			RootVisual = RootFrame;

			RootFrame.Navigated -= CompleteInitializePhoneApplication;

#if WINDOWS_PHONE_7
			if( e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh )
#else
			if( e.NavigationMode == NavigationMode.Reset )
#endif
			{
				while( RootFrame.RemoveBackEntry() != null )
				{
				}
			}
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
			var service = CreateApplicationService();

			if( service != null )
			{
				service.Launching += ( o, e ) => HandleApplicationLaunching( e );
				service.Activated += ( o, e ) => HandleApplicationActivated( e );
				service.Deactivated += ( o, e ) => HandleApplicationDeactivated( e );
				service.Closing += ( o, e ) => HandleApplicationClosing( e );

				ApplicationLifetimeObjects.Add( service );
			}
		}

		/// <summary>
		/// Creates the instance of application service.
		/// </summary>
		/// <returns>New instance of phone application service or <c>null</c> to omit application service registration.</returns>
		protected virtual PhoneApplicationService CreateApplicationService()
		{
			return new PhoneApplicationService();
		}

		/// <summary>
		/// Handles the application launching.
		/// </summary>
		/// <param name="e">The <see cref="LaunchingEventArgs" /> instance containing the event data.</param>
		protected virtual void HandleApplicationLaunching( LaunchingEventArgs e )
		{
		}

		/// <summary>
		/// Handles the application activated.
		/// </summary>
		/// <param name="e">The <see cref="ActivatedEventArgs" /> instance containing the event data.</param>
		protected virtual void HandleApplicationActivated( ActivatedEventArgs e )
		{
		}

		/// <summary>
		/// Handles the application deactivated.
		/// </summary>
		/// <param name="e">The <see cref="DeactivatedEventArgs" /> instance containing the event data.</param>
		protected virtual void HandleApplicationDeactivated( DeactivatedEventArgs e )
		{
		}

		/// <summary>
		/// Handles the application closing.
		/// </summary>
		/// <param name="e">The <see cref="ClosingEventArgs" /> instance containing the event data.</param>
		protected virtual void HandleApplicationClosing( ClosingEventArgs e )
		{
		}
		#endregion

		#region Navigation
		private void RootFrame_NavigationFailed( object sender, NavigationFailedEventArgs e )
		{
			HandleNavigationFailed( e );
		}

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
			var builder = new ContainerBuilder();

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
		#endregion
	}
}