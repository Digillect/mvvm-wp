using System;
using System.Diagnostics.Contracts;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	[ContractClassFor( typeof( IPageDecorator ) )]
	internal abstract class IPageDecoratorContract : IPageDecorator
	{
		#region Implementation of IPageDecorator
		/// <summary>
		///     Adds the decoration to the page.
		/// </summary>
		/// <param name="page">The page to decorate.</param>
		public void AddDecoration( Page page )
		{
			Contract.Requires<ArgumentNullException>( page != null );
		}

		/// <summary>
		///     Optionally removes the decoration from the page.
		/// </summary>
		/// <param name="page">The page to remove decoration from.</param>
		public void RemoveDecoration( Page page )
		{
			Contract.Requires<ArgumentNullException>( page != null );
		}
		#endregion
	}
}