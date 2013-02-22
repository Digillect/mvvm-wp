using System;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Interface that is used to perform uniform page decoration.
	/// </summary>
	/// <remarks>Decoration can be used, for example, to add certain UI elements to all pages
	/// (or to selected subset of pages) without the need to modify markup or code-behind classes.</remarks>
	public interface IPageDecorator
	{
		/// <summary>
		/// Adds the decoration to the page.
		/// </summary>
		/// <param name="page">The page to decorate.</param>
		void AddDecoration( Page page );
		/// <summary>
		/// Optionally removes the decoration from the page.
		/// </summary>
		/// <param name="page">The page to remove decoration from.</param>
		void RemoveDecoration( Page page );
	}
}
