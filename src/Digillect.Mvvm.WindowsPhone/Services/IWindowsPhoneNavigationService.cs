#region Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
// Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman).
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
using System.Diagnostics.Contracts;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Windows Phone extension to the <see cref="INavigationService" />.
	/// </summary>
	[ContractClass( typeof( IWindowsPhoneNavigationServiceContract ) )]
	public interface IWindowsPhoneNavigationService : INavigationService
	{
		/// <summary>
		///     Creates the snapshot of the current navigation state.
		/// </summary>
		/// <returns>
		///     Snapshot identifier that can be used to call <see cref="RollbackSnapshot(object)" />.
		/// </returns>
		object CreateSnapshot();

		/// <summary>
		///     Creates the snapshot of the current navigation state and sets a guard.
		/// </summary>
		/// <param name="guard">An action that will be called when current snapshot restored or destroyed.</param>
		/// <returns>
		///     Snapshot identifier that can be used to call <see cref="RollbackSnapshot(object)" />.
		/// </returns>
		object CreateSnapshot( Action<object> guard );

		/// <summary>
		///     Creates the snapshot of the current navigation state and sets a guard.
		/// </summary>
		/// <param name="guard">An action that will be called when current snapshot restored or destroyed.</param>
		/// <param name="tag">
		///     An arbitrary data to pass to the <paramref name="guard" />.
		/// </param>
		/// <returns>
		///     Snapshot identifier that can be used to call <see cref="RollbackSnapshot(object)" />.
		/// </returns>
		object CreateSnapshot( Action<object> guard, object tag );

		/// <summary>
		///     Rewinds navigation stack until it looks like when specified snapshot was taken.
		/// </summary>
		/// <param name="snapshotId">The snapshot id.</param>
		/// <returns>
		///     <c>true</c> if rollback was successful; otherwise <c>false</c>.
		/// </returns>
		bool RollbackSnapshot( object snapshotId );

		/// <summary>
		///     Rewinds navigation stack until it looks like when specified snapshot was taken
		///     and then navigate to the given view.
		/// </summary>
		/// <param name="snapshotId">The snapshot id.</param>
		/// <param name="viewName">Name of the view to navigate to after rollback.</param>
		/// <param name="parameters">The parameters to pass to view.</param>
		/// <returns>
		///     <c>true</c> if rollback was successful; otherwise <c>false</c>.
		/// </returns>
		bool RollbackSnapshot( object snapshotId, string viewName, XParameters parameters );
	}
}