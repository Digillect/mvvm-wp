using System;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	///     Instances of this class are used by MVVM infrastructure to support data binding.
	/// </summary>
	public class PageDataContext : ObservableObject, IDisposable
	{
		#region Delegates
		/// <summary>
		///     Factory that is used to create instances of context.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns>Instance of the context.</returns>
		public delegate PageDataContext Factory( Page page );
		#endregion

		private readonly Page _page;

		#region Constructors/Disposer
		/// <summary>
		///     Initializes a new instance of the <see cref="PageDataContext" /> class.
		/// </summary>
		/// <param name="page">The page used in this context.</param>
		public PageDataContext( Page page )
		{
			if( page == null )
			{
				throw new ArgumentNullException( "page" );
			}

			_page = page;
		}

		/// <summary>
		///     Releases unmanaged resources and performs other cleanup operations before the
		///     <see cref="PageDataContext" /> is reclaimed by garbage collection.
		/// </summary>
		~PageDataContext()
		{
			Dispose( false );
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		protected virtual void Dispose( bool disposing )
		{
		}
		#endregion

		#region Public Properties
		/// <summary>
		///     Gets the page.
		/// </summary>
		public Page Page
		{
			get { return _page; }
		}
		#endregion
	}
}