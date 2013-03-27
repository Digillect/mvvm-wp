using System;
using System.Diagnostics.Contracts;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	[ContractClassFor( typeof( IPageDecorationService ) )]
	internal abstract class IPageDecorationServiceContract : IPageDecorationService
	{
		#region Implementation of IPageDecorationService
		/// <summary>
		///     Performs decoration of the page.
		/// </summary>
		/// <param name="page">The page.</param>
		public void AddDecoration( Page page )
		{
			Contract.Requires<ArgumentNullException>( page != null );
		}

		/// <summary>
		///     Optionally removes decoration from the page.
		/// </summary>
		/// <param name="page">The page.</param>
		public void RemoveDecoration( Page page )
		{
			Contract.Requires<ArgumentNullException>( page != null );
		}
		#endregion
	}
}