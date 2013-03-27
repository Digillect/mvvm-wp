using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Implementers of this interface can inspect and change navigation operations.
	/// </summary>
	[ContractClass( typeof( INavigationHandlerContract ) )]
	public interface INavigationHandler
	{
		/// <summary>
		/// Handles the navigation operation.
		/// </summary>
		/// <param name="context">Navigation context.</param>
		/// <returns><c>true</c> to immediately continue processing of this context skipping any other handlers; otherwise <c>false</c>.</returns>
		Task<bool> HandleNavigation( NavigationContext context );
	}
}