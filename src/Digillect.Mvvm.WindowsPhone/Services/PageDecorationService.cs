using System;
using System.Collections.Generic;

using Autofac;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Default implementation of <see cref="IPageDecorationService"/>.
	/// </summary>
	public sealed class PageDecorationService : IPageDecorationService
	{
		#region Add/Remove Page decoration
		/// <summary>
		/// Performs decoration of the page.
		/// </summary>
		/// <param name="page">The page.</param>
		public void AddDecoration( PhoneApplicationPage page )
		{
			if( page == null )
				throw new ArgumentNullException( "page" );

			foreach( var decorator in page.Scope.Resolve<IEnumerable<IPageDecorator>>() )
				decorator.AddDecoration( page );
		}

		/// <summary>
		/// Optionally removes decoration from the page.
		/// </summary>
		/// <param name="page">The page.</param>
		public void RemoveDecoration( PhoneApplicationPage page )
		{
			if( page == null )
				throw new ArgumentNullException( "page" );

			foreach( var decorator in page.Scope.Resolve<IEnumerable<IPageDecorator>>() )
				decorator.RemoveDecoration( page );
		}
		#endregion
	}
}
