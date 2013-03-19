using System.Threading.Tasks;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Supports authentication process
	/// </summary>
	public interface IAuthenticationServiceContext
	{
		#region Public Properties
		/// <summary>
		///     Gets the name of the default initial authentication view.
		/// </summary>
		/// <value>
		///     The name of the view.
		/// </value>
		string AuthenticationViewName { get; }

		/// <summary>
		///     Gets the initial authentication view parameters.
		/// </summary>
		/// <value>
		///     The initial authentication view parameters.
		/// </value>
		Parameters AuthenticationViewParameters { get; }
		#endregion

		/// <summary>
		///     Determines whether user has been authenticated.
		/// </summary>
		/// <returns>Task that can be awaited to get value, indicating wether current user has been authenticated.</returns>
		Task<bool> IsAuthenticated();
	}
}