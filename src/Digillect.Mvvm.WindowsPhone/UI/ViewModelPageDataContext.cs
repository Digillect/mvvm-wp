using System;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Instances of this class are used by <see cref="Digillect.Mvvm.UI.ViewModelPage{TViewModel}"/> and descendants to provide data binding support.
	/// </summary>
	public class ViewModelPageDataContext : PageDataContext
	{
		/// <summary>
		/// Factory that is used to create instances of context.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>Instance of context.</returns>
		[CLSCompliant( false )]
		public new delegate ViewModelPageDataContext Factory( PhoneApplicationPage page, ViewModel viewModel );

		#region Constructors/Disposer
		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelPageDataContext"/> class.
		/// </summary>
		/// <param name="page">The page used in this context.</param>
		/// <param name="viewModel">The view model used in this context.</param>
		[CLSCompliant( false )]
		public ViewModelPageDataContext( PhoneApplicationPage page, ViewModel viewModel )
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
		/// Gets the view model.
		/// </summary>
		[CLSCompliant( false )]
		public ViewModel ViewModel { get; private set; }
		#endregion
	}
}
