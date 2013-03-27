using System;
using System.Diagnostics.Contracts;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Windows Phone extension to the <see cref="INavigationService"/>.
	/// </summary>
	[ContractClass( typeof( IWindowsPhoneNavigationServiceContract ) )]
	public interface IWindowsPhoneNavigationService : INavigationService
	{
		/// <summary>
		/// Creates the snapshot of the current navigation state.
		/// </summary>
		/// <returns>Snapshot identifier that can be used to call <see cref="RollbackSnapshot(object)"/>.</returns>
		object CreateSnapshot();
		/// <summary>
		/// Creates the snapshot of the current navigation state and sets a guard.
		/// </summary>
		/// <param name="guard">An action that will be called when current snapshot restored or destroyed.</param>
		/// <returns>Snapshot identifier that can be used to call <see cref="RollbackSnapshot(object)"/>.</returns>
		object CreateSnapshot( Action<object> guard );
		/// <summary>
		/// Creates the snapshot of the current navigation state and sets a guard.
		/// </summary>
		/// <param name="guard">An action that will be called when current snapshot restored or destroyed.</param>
		/// <param name="tag">An arbitrary data to pass to the <paramref name="guard"/>.</param>
		/// <returns>Snapshot identifier that can be used to call <see cref="RollbackSnapshot(object)"/>.</returns>
		object CreateSnapshot( Action<object> guard, object tag );
		/// <summary>
		/// Rewinds navigation stack until it looks like when specified snapshot was taken.
		/// </summary>
		/// <param name="snapshotId">The snapshot id.</param>
		/// <returns><c>true</c> if rollback was successful; otherwise <c>false</c>.</returns>
		bool RollbackSnapshot( object snapshotId );
		/// <summary>
		/// Rewinds navigation stack until it looks like when specified snapshot was taken
		/// and then navigate to the given view.
		/// </summary>
		/// <param name="snapshotId">The snapshot id.</param>
		/// <param name="viewName">Name of the view to navigate to after rollback.</param>
		/// <param name="parameters">The parameters to pass to view.</param>
		/// <returns>
		///   <c>true</c> if rollback was successful; otherwise <c>false</c>.
		/// </returns>
		bool RollbackSnapshot( object snapshotId, string viewName, Parameters parameters );
	}
}