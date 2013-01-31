using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Provides infrastructure for page backed up with <see cref="Digillect.Mvvm.ViewModel"/>.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public class ViewModelPage<TViewModel> : PhoneApplicationPage
		where TViewModel : ViewModel
	{
		private TViewModel viewModel;

		#region Public Properties
		/// <summary>
		/// Gets the view model.
		/// </summary>
		public TViewModel ViewModel
		{
			get { return viewModel; }
		}
		#endregion

		#region Page lifecycle
		/// <summary>
		/// This method is called when page is visited for the very first time. You should perform
		/// initialization and create one-time initialized resources here.
		/// </summary>
		protected override void OnPageCreated()
		{
			base.OnPageCreated();

			if( !IsInDesignMode )
			{
				InitialLoadData();
			}
		}

		/// <summary>
		/// This method is called when page navigated after application has been resurrected from thombstombed state.
		/// Use <see cref="Microsoft.Phone.Controls.PhoneApplicationPage.State"/> property to restore state.
		/// </summary>
		protected override void OnPageResurrected()
		{
			base.OnPageResurrected();

			if( !IsInDesignMode )
			{
				InitialLoadData();
			}
		}

		/// <summary>
		/// Creates the view model.
		/// </summary>
		/// <returns>View model for this page.</returns>
		protected virtual TViewModel CreateViewModel()
		{
			return Scope.Resolve<TViewModel>();
		}

		/// <summary>
		/// Creates data context to be set for the page. Override to create your own data context.
		/// </summary>
		/// <returns>
		/// Data context that will be set to <see cref="System.Windows.FrameworkElement.DataContext"/> property.
		/// </returns>
		protected override PageDataContext CreateDataContext()
		{
			this.viewModel = CreateViewModel();

			return Scope.Resolve<ViewModelPageDataContext.Factory>()( this, this.viewModel );
		}
		#endregion

		#region InitialLoadData
		/// <summary>
		/// This method is called to perform initial load of data when page is created for the first time
		/// or resurrected from thombstombing if no other state-saving scenario exists. Default implementation
		/// does nothing.
		/// </summary>
		protected virtual void InitialLoadData()
		{
		}
		#endregion
	}
}
