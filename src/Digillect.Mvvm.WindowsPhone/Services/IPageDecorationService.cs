using System.Diagnostics.Contracts;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Service that is used to handle page decoration and decorators management.
	/// </summary>
	[ContractClass( typeof( IPageDecorationServiceContract ) )]
	public interface IPageDecorationService
	{
		/// <summary>
		///     Performs decoration of the page.
		/// </summary>
		/// <param name="page">The page.</param>
		void AddDecoration( Page page );

		/// <summary>
		///     Optionally removes decoration from the page.
		/// </summary>
		/// <param name="page">The page.</param>
		void RemoveDecoration( Page page );
	}
}