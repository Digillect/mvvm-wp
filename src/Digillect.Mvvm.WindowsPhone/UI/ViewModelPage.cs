using System;
using System.Threading.Tasks;

using Autofac;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Provides infrastructure for page backed up with <see cref="Digillect.Mvvm.ViewModel" />.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the page model.</typeparam>
	public class ViewModelPage<TViewModel> : Page
		where TViewModel : ViewModel
	{
		private TViewModel _viewModel;

		#region Public Properties
		/// <summary>
		///     Gets the page model.
		/// </summary>
		public TViewModel ViewModel
		{
			get { return _viewModel; }
		}
		#endregion

		#region Page lifecycle
		/// <summary>
		///     This method is called when page is visited for the very first time. You should perform
		///     initialization and create one-time initialized resources here.
		/// </summary>
		protected override void OnPageCreated()
		{
			base.OnPageCreated();

			if( !IsInDesignMode )
			{
				InternalLoadData( DataLoadReason.New );
			}
		}

		/// <summary>
		///     This method is called when page navigated after application has been resurrected from thombstombed state.
		///     Use <see cref="Microsoft.Phone.Controls.PhoneApplicationPage.State" /> property to restore state.
		/// </summary>
		protected override void OnPageResurrected()
		{
			base.OnPageResurrected();

			if( !IsInDesignMode )
			{
				InternalLoadData( DataLoadReason.Resurrection );
			}
		}

		/// <summary>
		///     This method is called when page is returned from being Dormant. All resources are preserved,
		///     so most of the time you should just ignore this event.
		/// </summary>
		protected override void OnPageAwaken()
		{
			base.OnPageAwaken();

			if( !IsInDesignMode )
			{
				InternalLoadData( DataLoadReason.Awakening );
			}
		}

		/// <summary>
		///     Creates the page model.
		/// </summary>
		/// <returns>page model for this page.</returns>
		protected virtual TViewModel CreateViewModel()
		{
			return Scope.Resolve<TViewModel>();
		}

		/// <summary>
		///     Creates data context to be set for the page. Override to create your own data context.
		/// </summary>
		/// <returns>
		///     Data context that will be set to <see cref="System.Windows.FrameworkElement.DataContext" /> property.
		/// </returns>
		protected override PageDataContext CreateDataContext()
		{
			_viewModel = CreateViewModel();

			return Scope.Resolve<ViewModelPageDataContext.Factory>()( this, _viewModel );
		}
		#endregion

		#region Data Loading
		/// <summary>
		/// Starts the process of loading data.
		/// </summary>
		protected void LoadData()
		{
			InternalLoadData( DataLoadReason.Manual );
		}

		private async void InternalLoadData( DataLoadReason reason )
		{
			var context = (ViewModelPageDataContext) DataContext;

			if( reason != DataLoadReason.Awakening || !context.DataIsLoaded )
			{
				var task = LoadData( reason );

				if( task != null )
				{
					try
					{
						var session = await task;

						OnLoadDataComplete( session );

						context.DataIsLoaded = true;
					}
					catch( Exception ex )
					{
						OnLoadDataFailed( ex );
					}
				}
			}
		}

		/// <summary>
		///     This method is called to create data loading task.
		/// </summary>
		protected virtual Task<Session> LoadData( DataLoadReason reason )
		{
			return null;
		}

		/// <summary>
		/// Called when data loading process successfully completes.
		/// </summary>
		/// <param name="session">The session.</param>
		protected virtual void OnLoadDataComplete( Session session )
		{
		}

		/// <summary>
		/// Called when data loading process fails.
		/// </summary>
		/// <param name="ex">Reason of the failure.</param>
		protected virtual void OnLoadDataFailed( Exception ex )
		{
		}
		#endregion
	}
}