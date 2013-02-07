using System;
using System.Windows;
using System.Windows.Input;

using Autofac;

using Digillect.Mvvm.Services;

namespace Digillect.Mvvm.UI
{
	#region NavigationCommand
	/// <summary>
	/// Command that performs navigation to the specified view.
	/// </summary>
	public class NavigationCommand : ICommand
	{
		private readonly string _view;
		private readonly Func<bool> _canNavigate;
		private readonly Func<Parameters> _parametersProvider;

		#region Constructors/Disposer
		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		public NavigationCommand( string view )
		{
			_view = view;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="canNavigate">Function that determines whether it is possible to navigate to the specified view.</param>
		public NavigationCommand( string view, Func<bool> canNavigate )
		{
			_view = view;
			_canNavigate = canNavigate;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="parameters">Parameters to pass to the target view.</param>
		public NavigationCommand( string view, Parameters parameters )
		{
			_view = view;
			_parametersProvider = () => parameters;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="canNavigate">Function that determines whether it is possible to navigate to the specified view.</param>
		/// <param name="parameters">Parameters to pass to the target view.</param>
		public NavigationCommand( string view, Func<bool> canNavigate, Parameters parameters )
		{
			_view = view;
			_canNavigate = canNavigate;
			_parametersProvider = () => parameters;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="parametersProvider">Function that returns parameters to pass to the target view.</param>
		public NavigationCommand( string view, Func<Parameters> parametersProvider )
		{
			_view = view;
			_parametersProvider = parametersProvider;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="canNavigate">Function that determines whether it is possible to navigate to the specified view.</param>
		/// <param name="parametersProvider">Function that returns parameers to pass to the target view.</param>
		public NavigationCommand( string view, Func<bool> canNavigate, Func<Parameters> parametersProvider )
		{
			_view = view;
			_canNavigate = canNavigate;
			_parametersProvider = parametersProvider;
		}
		#endregion

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises event that indicates that <see cref="CanExecute"/> return value has been changed.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public void RaiseCanExecuteChanged()
		{
			var handler = CanExecuteChanged;

			if( handler != null )
			{
				handler( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command. Always ignored in this implementation.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		public bool CanExecute( object parameter )
		{
			return _canNavigate == null || _canNavigate();
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command. Always ignored in this implementation.</param>
		public void Execute( object parameter )
		{
			if( CanExecute( parameter ) )
			{
				var navigationService = ((PhoneApplication) Application.Current).Scope.Resolve<INavigationService>();

				Parameters parameters = _parametersProvider == null ? null : _parametersProvider();

				navigationService.Navigate( _view, parameters );
			}
		}
	}
	#endregion
	#region NavigationCommand
	/// <summary>
	/// Command that performs navigation to the specified view.
	/// </summary>
	public class NavigationCommand<T> : ICommand
	{
		private readonly string _view;
		private readonly Func<T, bool> _canNavigate;
		private readonly Func<T, Parameters> _parametersProvider;

		#region Constructors/Disposer
		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		public NavigationCommand( string view )
		{
			_view = view;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="canNavigate">Function that determines whether it is possible to navigate to the specified view.</param>
		public NavigationCommand( string view, Func<T, bool> canNavigate )
		{
			_view = view;
			_canNavigate = canNavigate;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="parameters">Parameters to pass to the target view.</param>
		public NavigationCommand( string view, Parameters parameters )
		{
			_view = view;
			_parametersProvider = commandParameter => parameters;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="canNavigate">Function that determines whether it is possible to navigate to the specified view.</param>
		/// <param name="parameters">Parameters to pass to the target view.</param>
		public NavigationCommand( string view, Func<T, bool> canNavigate, Parameters parameters )
		{
			_view = view;
			_canNavigate = canNavigate;
			_parametersProvider = commandParameter => parameters;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="parametersProvider">Function that returns parameters to pass to the target view.</param>
		public NavigationCommand( string view, Func<T, Parameters> parametersProvider )
		{
			_view = view;
			_parametersProvider = parametersProvider;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NavigationCommand" /> class.
		/// </summary>
		/// <param name="view">Target view.</param>
		/// <param name="canNavigate">Function that determines whether it is possible to navigate to the specified view.</param>
		/// <param name="parametersProvider">Function that returns parameers to pass to the target view.</param>
		public NavigationCommand( string view, Func<T, bool> canNavigate, Func<T, Parameters> parametersProvider )
		{
			_view = view;
			_canNavigate = canNavigate;
			_parametersProvider = parametersProvider;
		}
		#endregion

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises event that indicates that <see cref="CanExecute"/> return value has been changed.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public void RaiseCanExecuteChanged()
		{
			var handler = CanExecuteChanged;

			if( handler != null )
			{
				handler( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command. Always ignored in this implementation.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		public bool CanExecute( object parameter )
		{
			return _canNavigate == null || _canNavigate( (T) parameter );
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command. Always ignored in this implementation.</param>
		public void Execute( object parameter )
		{
			if( CanExecute( parameter ) )
			{
				var navigationService = ((PhoneApplication) Application.Current).Scope.Resolve<INavigationService>();

				Parameters parameters = _parametersProvider == null ? null : _parametersProvider( (T) parameter );

				navigationService.Navigate( _view, parameters );
			}
		}
	}
	#endregion
}
