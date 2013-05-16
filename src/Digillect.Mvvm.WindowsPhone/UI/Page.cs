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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;

using Autofac;

using Digillect.Mvvm.Services;

using Microsoft.Phone.Controls;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	///     Base for application pages.
	/// </summary>
	public class Page : PhoneApplicationPage, INotifyPropertyChanged
	{
		private const string RessurectionMark = "__mark$mark__";

		private XParameters _viewParameters = XParameters.Empty;
		private ILifetimeScope _scope;

		#region Constructor
		/// <summary>
		///     Initializes a new instance of the <see cref="Page" /> class.
		/// </summary>
		protected Page()
		{
			Language = XmlLanguage.GetLanguage( Thread.CurrentThread.CurrentCulture.Name );
		}
		#endregion

		#region Public Properties
		/// <summary>
		///     Gets the IoC lifetime scope.
		/// </summary>
		public ILifetimeScope Scope
		{
			get { return _scope; }
		}

		/// <summary>
		///     Gets the parameters passed to this view.
		/// </summary>
		protected XParameters ViewParameters
		{
			get { return _viewParameters; }
		}
		#endregion

		#region Navigation handling
		/// <summary>
		///     Called when a page becomes the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			if( _scope == null )
			{
				_scope = ((PhoneApplication) Application.Current).Scope.BeginLifetimeScope();

				ParseParameters();

				DataContext = CreateDataContext();

				if( State.ContainsKey( RessurectionMark ) )
				{
					OnPageResurrected();
				}
				else
				{
					OnPageCreated();
				}

				IPageDecorationService pageDecorationService;

				if( Scope.TryResolve( out pageDecorationService ) )
				{
					pageDecorationService.AddDecoration( this );
				}
			}
			else
			{
				OnPageAwaken();
			}
		}

		/// <summary>
		///     Called when a page is no longer the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedFrom( NavigationEventArgs e )
		{
			if( e.NavigationMode == NavigationMode.Back )
			{
				OnPageDestroyed();

				if( _scope != null )
				{
					IPageDecorationService pageDecorationService = null;

					if( Scope.TryResolve( out pageDecorationService ) )
					{
						pageDecorationService.RemoveDecoration( this );
					}

					_scope.Dispose();
					_scope = null;
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

		#region Events and event raisers
		/// <summary>
		///     This method is called when page is visited for the very first time. You should perform
		///     initialization and create one-time initialized resources here.
		/// </summary>
		protected virtual void OnPageCreated()
		{
		}

		/// <summary>
		///     This method is called when page is returned from being Dormant. All resources are preserved,
		///     so most of the time you should just ignore this event.
		/// </summary>
		protected virtual void OnPageAwaken()
		{
		}

		/// <summary>
		///     This method is called when page navigated after application has been resurrected from thombstombed state.
		///     Use <see cref="Microsoft.Phone.Controls.PhoneApplicationPage.State" /> property to restore state.
		/// </summary>
		protected virtual void OnPageResurrected()
		{
		}

		/// <summary>
		///     This method is called when navigation outside of the page occures.
		/// </summary>
		protected virtual void OnPageAsleep()
		{
		}

		/// <summary>
		///     This method is called when page is being destroyed, usually after user presses Back key.
		/// </summary>
		protected virtual void OnPageDestroyed()
		{
		}
		#endregion

		/// <summary>
		///     Creates data context to be set for the page. Override to create your own data context.
		/// </summary>
		/// <returns>
		///     Data context that will be set to <see cref="System.Windows.FrameworkElement.DataContext" /> property.
		/// </returns>
		protected virtual object CreateDataContext()
		{
			return this;
		}
		#endregion

		#region IsInDesignMode
		private static bool? isInDesignMode;

		/// <summary>
		///     Gets a value indicating whether this page is in design mode.
		/// </summary>
		/// <value>
		///     <c>true</c> if this page is in design mode; otherwise, <c>false</c>.
		/// </value>
		public static bool IsInDesignMode
		{
			get
			{
				if( !isInDesignMode.HasValue )
				{
					isInDesignMode = DesignerProperties.IsInDesignTool;
				}

				return isInDesignMode.Value;
			}
		}
		#endregion

		#region Parameters Parsing
		private void ParseParameters()
		{
			var queryString = new Dictionary<string, string>( NavigationContext.QueryString, StringComparer.OrdinalIgnoreCase );
			var builder = _viewParameters.ToBuilder();

			ParseParameters( queryString, builder );

			_viewParameters = builder.ToImmutable();
		}

		/// <summary>
		///     Parses the parameters.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <param name="builder">Parameters builder to add parameters to.</param>
		/// <exception cref="System.ArgumentException"></exception>
		protected virtual void ParseParameters( IDictionary<string, string> queryString, XParameters.Builder builder )
		{
			Type pageType = GetType();

			foreach( var attribute in pageType.GetCustomAttributes( typeof( ViewParameterAttribute ), true ).Cast<ViewParameterAttribute>() )
			{
				string stringValue;
				object parameterValue = null;

				if( queryString.TryGetValue( attribute.ParameterName, out stringValue ) )
				{
					try
					{
						parameterValue = ParametersSerializer.DecodeValue( stringValue, attribute.ParameterType );
					}
					catch( Exception ex )
					{
						throw new ViewParameterException( string.Format( "View '{0}' can not parse parameter '{1}' of type '{2}'.", pageType, attribute.ParameterName, attribute.ParameterType ), ex );
					}

					if( parameterValue != null )
					{
						builder.AddValue( attribute.ParameterName, parameterValue );
					}

					queryString.Remove( attribute.ParameterName );
				}

				if( parameterValue == null )
				{
					if( attribute.Required )
					{
						throw new ViewParameterException( string.Format( "View '{0}' requires parameter '{1}' of type '{2}' but it was not passed.", pageType, attribute.ParameterName, attribute.ParameterType ) );
					}
				}
			}
		}
		#endregion

		#region INotifyPropertyChanged implementation

		#region Events and event raisers
		/// <summary>
		///     Raises the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="e">
		///     The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.
		/// </param>
		protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
		{
			if( PropertyChanged != null )
			{
				PropertyChanged( this, e );
			}
		}
		#endregion

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///     Raises the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
#if NET45
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required for the CallerMemberName attribute.")]
		protected void OnPropertyChanged( [CallerMemberName] string propertyName = null )
#else
		protected void OnPropertyChanged( string propertyName )
#endif
		{
			OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		///     Checks if a property already matches a desired value.  Sets the property and
		///     notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="location">The variable to set to the specified value.</param>
		/// <param name="value">
		///     The value to which the <paramref name="location" /> parameter is set.
		/// </param>
		/// <param name="propertyName">Name of the property used to notify listeners.</param>
		/// <returns>
		///     <c>True</c> if the value has changed; <c>false</c> if the <paramref name="location" /> matches (by equality) the
		///     <paramref
		///         name="value" />
		///     .
		/// </returns>
		/// <remarks>
		///     <b>.NET 4.5.</b> <paramref name="propertyName" /> is optional and can be provided automatically
		///     when invoked from compilers which support the <c>CallerMemberName</c> attribute.
		/// </remarks>
		[SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#" )]
#if NET45
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required for the CallerMemberName attribute.")]
		protected bool SetProperty<T>(ref T location, T value, [CallerMemberName] string propertyName = null)
#else
		protected bool SetProperty<T>( ref T location, T value, string propertyName )
#endif
		{
			if( Equals( location, value ) )
			{
				return false;
			}

			location = value;

			if( propertyName != null )
			{
				OnPropertyChanged( propertyName );
			}

			return true;
		}
		#endregion
	}
}