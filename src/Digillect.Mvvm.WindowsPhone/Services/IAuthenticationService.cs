using System.Threading.Tasks;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Orchestrates authentication process.
	/// </summary>
	public interface IAuthenticationService
	{
		/// <summary>
		/// Gets a value indicating whether authentication is in progress.
		/// </summary>
		/// <value>
		/// <c>true</c> if authentication is in progress; otherwise, <c>false</c>.
		/// </value>
		bool AuthenticationInProgress { get; }

		/// <summary>
		/// Starts the authentication.
		/// </summary>
		Task StartAuthentication();
		/// <summary>
		/// Starts the authentication.
		/// </summary>
		/// <param name="initialViewName">Name of the initial view in the authentication flow.</param>
		/// <param name="parameters">Parameters for the initial view.</param>
		Task StartAuthentication( string initialViewName, Parameters parameters );

		/// <summary>
		/// Completes the authentication.
		/// </summary>
		void CompleteAuthentication();
		/// <summary>
		/// Cancels the authentication.
		/// </summary>
		void CancelAuthentication();
	}
}