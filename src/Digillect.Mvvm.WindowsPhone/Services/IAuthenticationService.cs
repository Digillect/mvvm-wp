#region Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2014 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
#endregion

using System;
using System.Threading.Tasks;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Orchestrates authentication process.
	/// </summary>
	public interface IAuthenticationService
	{
		#region Public Properties
		/// <summary>
		///     Gets a value indicating whether authentication is in progress.
		/// </summary>
		/// <value>
		///     <c>true</c> if authentication is in progress; otherwise, <c>false</c>.
		/// </value>
		bool AuthenticationInProgress { get; }
		#endregion

		#region Public methods
		/// <summary>
		///     Cancels the authentication.
		/// </summary>
		void CancelAuthentication();

		/// <summary>
		///     Completes the authentication.
		/// </summary>
		void CompleteAuthentication();

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		Task StartAuthentication();

		/// <summary>
		///     Starts the authentication and performs an action when authentication completes.
		/// </summary>
		/// <param name="performWhenAuthenticated">Action to perform when authentication successfully completes.</param>
		Task StartAuthentication( Action performWhenAuthenticated );

		/// <summary>
		///     Starts the authentication.
		/// </summary>
		/// <param name="initialViewName">Name of the initial view in the authentication flow.</param>
		/// <param name="parameters">Parameters for the initial view.</param>
		Task StartAuthentication( string initialViewName, XParameters parameters );
		#endregion
	}
}