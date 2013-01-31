using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

using Digillect.Mvvm.Services;

using Autofac;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Base for application pages.
	/// </summary>
	public class PhoneApplicationPage : Microsoft.Phone.Controls.PhoneApplicationPage
	{
		private const string RessurectionMark = "__mark$mark__";

		private ILifetimeScope scope;
		private readonly Parameters _parameters = new Parameters();

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PhoneApplicationPage"/> class.
		/// </summary>
		public PhoneApplicationPage()
		{
			this.Language = XmlLanguage.GetLanguage( Thread.CurrentThread.CurrentCulture.Name );
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the current application.
		/// </summary>
		public PhoneApplication CurrentApplication
		{
			get { return (PhoneApplication) Application.Current; }
		}

		/// <summary>
		/// Gets the IoC lifetime scope.
		/// </summary>
		public ILifetimeScope Scope
		{
			get { return this.scope; }
		}

		/// <summary>
		/// Gets the page parameters.
		/// </summary>
		/// <value>
		/// The parameters.
		/// </value>
		[CLSCompliant( false )]
		public Parameters Parameters
		{
			get { return _parameters; }
		}
		#endregion

		#region Navigation handling
		/// <summary>
		/// Called when a page becomes the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedTo( System.Windows.Navigation.NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			if( this.scope == null )
			{
				this.scope = CurrentApplication.Scope.BeginLifetimeScope();

				ParseParameters();
				this.DataContext = CreateDataContext();

				if( State.ContainsKey( RessurectionMark ) )
					OnPageResurrected();
				else
					OnPageCreated();

				IPageDecorationService pageDecorationService = null;

				if( this.Scope.TryResolve<IPageDecorationService>( out pageDecorationService ) )
				{
					pageDecorationService.AddDecoration( this );
				}
			}
		}

		/// <summary>
		/// Called when a page is no longer the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedFrom( System.Windows.Navigation.NavigationEventArgs e )
		{
			if( e.NavigationMode == System.Windows.Navigation.NavigationMode.Back )
			{
				OnPageDestroyed();

				if( this.scope != null )
				{
					IPageDecorationService pageDecorationService = null;

					if( this.Scope.TryResolve<IPageDecorationService>( out pageDecorationService ) )
					{
						pageDecorationService.RemoveDecoration( this );
					}

					this.scope.Dispose();
					this.scope = null;
				}
			}
			else
			{
				State[RessurectionMark] = true;
				OnPageAsleep();
			}

			base.OnNavigatedFrom( e );
		}
		#endregion

		#region Page Lifecycle handlers
		/// <summary>
		/// Creates data context to be set for the page. Override to create your own data context.
		/// </summary>
		/// <returns>Data context that will be set to <see cref="System.Windows.FrameworkElement.DataContext"/> property.</returns>
		protected virtual PageDataContext CreateDataContext()
		{
			return this.scope.Resolve<PageDataContext.Factory>()( this );
		}

		/// <summary>
		/// This method is called when page is visited for the very first time. You should perform
		/// initialization and create one-time initialized resources here.
		/// </summary>
		protected virtual void OnPageCreated()
		{
		}

		/// <summary>
		/// This method is called when page is returned from being Dormant. All resources are preserved,
		/// so most of the time you should just ignore this event.
		/// </summary>
		protected virtual void OnPageAwaken()
		{
		}

		/// <summary>
		/// This method is called when page navigated after application has been resurrected from thombstombed state.
		/// Use <see cref="Microsoft.Phone.Controls.PhoneApplicationPage.State"/> property to restore state.
		/// </summary>
		protected virtual void OnPageResurrected()
		{
		}

		/// <summary>
		/// This method is called when navigation outside of the page occures.
		/// </summary>
		protected virtual void OnPageAsleep()
		{
		}

		/// <summary>
		/// This method is called when page is being destroyed, usually after user presses Back key.
		/// </summary>
		protected virtual void OnPageDestroyed()
		{
		}
		#endregion

		#region IsInDesignMode
		private static bool? isInDesignMode;

		/// <summary>
		/// Gets a value indicating whether this page is in design mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this page is in design mode; otherwise, <c>false</c>.
		/// </value>
		public static bool IsInDesignMode
		{
			get
			{
				if( !isInDesignMode.HasValue )
					isInDesignMode = System.ComponentModel.DesignerProperties.IsInDesignTool;

				return isInDesignMode.Value;
			}
		}
		#endregion

		#region Parameters Parsing
		private void ParseParameters()
		{
			var queryString = new Dictionary<string, string>( NavigationContext.QueryString, StringComparer.OrdinalIgnoreCase );

			ParseParameters( queryString );
		}

		/// <summary>
		/// Parses the parameters.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <exception cref="System.ArgumentException"></exception>
		protected virtual void ParseParameters( IDictionary<string, string> queryString )
		{
			var pageType = GetType();

			foreach( var attribute in pageType.GetCustomAttributes( typeof( ViewParameterAttribute ), true ).Cast<ViewParameterAttribute>() )
			{
				string stringValue = null;
				object parameterValue = null;

				if( queryString.TryGetValue( attribute.ParameterName, out stringValue ) )
				{
					parameterValue = Digillect.Mvvm.Services.NavigationService.DecodeValue( stringValue, attribute.ParameterType );

					if( parameterValue != null )
					{
						_parameters.Add( attribute.ParameterName, parameterValue );
					}

					queryString.Remove( attribute.ParameterName );
				}

				if( parameterValue == null )
				{
					if( attribute.Required )
					{
						throw new ArgumentException( string.Format( "Page {0} requires argument {1} of type {2}.", pageType, attribute.ParameterName, attribute.ParameterType ), attribute.ParameterName );
					}
				}
			}
		}
		#endregion
	}
}
