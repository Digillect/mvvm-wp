#region Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
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
using System.Threading.Tasks;

using Autofac;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	///     Provides infrastructure for page backed up with <see cref="Digillect.Mvvm.ViewModel" />.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the page model.</typeparam>
	public class ViewModelPage<TViewModel> : Page
		where TViewModel : ViewModel
	{
		private bool _dataIsLoaded;
		private Session _session;
		private TViewModel _viewModel;

		#region Public Properties
		/// <summary>
		///     Gets the page model.
		/// </summary>
		public TViewModel ViewModel
		{
			get { return _viewModel; }
		}

		/// <summary>
		///     Gets or sets a value indicating whether data for the view is loaded.
		/// </summary>
		/// <value>
		///     <c>true</c> if data is loaded; otherwise, <c>false</c>.
		/// </value>
		public bool DataIsLoaded
		{
			get { return _dataIsLoaded; }
			private set { SetProperty( ref _dataIsLoaded, value, "DataIsLoaded" ); }
		}
		#endregion

		#region Page lifecycle
		/// <summary>
		///     Called when data loading process successfully completes.
		/// </summary>
		/// <param name="session">The session.</param>
		protected virtual void OnDataLoadComplete( Session session )
		{
		}

		/// <summary>
		///     Called when data loading process fails.
		/// </summary>
		/// <param name="session">Session that failed to load.</param>
		/// <param name="ex">Reason of the failure.</param>
		protected virtual void OnDataLoadFailed( Session session, Exception ex )
		{
		}

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
		///     This method is called when navigation outside of the page occures.
		/// </summary>
		protected override void OnPageAsleep()
		{
			base.OnPageAsleep();

			Session session = _session;

			_session = null;

			if( session != null )
			{
				session.Cancel();
			}
		}
		#endregion

		#region ViewModel creation
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
		protected override object CreateDataContext()
		{
			_viewModel = CreateViewModel();

			return base.CreateDataContext();
		}
		#endregion

		#region Data Loading
		/// <summary>
		///     Starts the process of loading data.
		/// </summary>
		protected void LoadData()
		{
			InternalLoadData( DataLoadReason.Manual );
		}

		private async void InternalLoadData( DataLoadReason reason )
		{
			if( reason != DataLoadReason.Awakening || !_dataIsLoaded )
			{
				var session = _session = CreateDataSession( reason );

				if( session == null )
				{
					session = _session = await CreateDataSessionAsync( reason );
				}

				if( session != null )
				{
					try
					{
						await _viewModel.Load( session );

						OnDataLoadComplete( session );

						DataIsLoaded = true;
					}
					catch( Exception ex )
					{
						OnDataLoadFailed( session, ex );
					}
					finally
					{
						_session = null;
					}
				}
			}
		}

		/// <summary>
		///     This method is called to create data loading session.
		/// </summary>
		/// <param name="reason">The reason to load page data.</param>
		/// <returns>Session that should be used to load page data.</returns>
		protected virtual Session CreateDataSession( DataLoadReason reason )
		{
			return ViewModel.CreateSession( ViewParameters );
		}

		/// <summary>
		///     This method is called to asynchronously create data loading session.
		/// </summary>
		/// <param name="reason">The reason to load page data.</param>
		/// <returns>Task that will produce session that should be used to load page data.</returns>
		protected virtual Task<Session> CreateDataSessionAsync( DataLoadReason reason )
		{
			return null;
		}
		#endregion
	}
}