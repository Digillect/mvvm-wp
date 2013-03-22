using System;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Instances of this class are used by <see cref="ViewModelPage{TViewModel}"/> and descendants to provide data binding support.
	/// </summary>
	public class ViewModelPageDataContext : PageDataContext
	{
		private bool _dataIsLoaded;

		#region Constructors/Disposer
		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelPageDataContext"/> class.
		/// </summary>
		/// <param name="page">The page used in this context.</param>
		/// <param name="viewModel">The page model used in this context.</param>
		[CLSCompliant( false )]
		public ViewModelPageDataContext( Page page, ViewModel viewModel )
			: base( page )
		{
			if( page == null )
			{
				throw new ArgumentNullException( "page" );
			}

			ViewModel = viewModel;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the page model.
		/// </summary>
		[CLSCompliant( false )]
		public ViewModel ViewModel { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether ViewModel has loaded initial set of data.
		/// </summary>
		/// <value>
		///   <c>true</c> if data is loaded; otherwise, <c>false</c>.
		/// </value>
		public bool DataIsLoaded
		{
			get { return _dataIsLoaded; }
			set { SetProperty( ref _dataIsLoaded, value, "DataIsLoaded" ); }
		}
		#endregion
	}
}
