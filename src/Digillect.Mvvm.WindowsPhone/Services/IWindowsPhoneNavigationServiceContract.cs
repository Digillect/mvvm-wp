using System;
using System.Diagnostics.Contracts;

namespace Digillect.Mvvm.Services
{
	[ContractClassFor( typeof( IWindowsPhoneNavigationService ) )]
	internal abstract class IWindowsPhoneNavigationServiceContract : IWindowsPhoneNavigationService
	{
		#region Implementation of IWindowsPhoneNavigationService
		public object CreateSnapshot()
		{
			Contract.Ensures( Contract.Result<object>() != null );

			return null;
		}

		public object CreateSnapshot( Action<object> guard )
		{
			Contract.Ensures( Contract.Result<object>() != null );

			return null;
		}

		public object CreateSnapshot( Action<object> guard, object tag )
		{
			Contract.Ensures( Contract.Result<object>() != null );

			return null;
		}

		public bool RollbackSnapshot( object snapshotId )
		{
			Contract.Requires<ArgumentNullException>( snapshotId != null );

			return false;
		}

		public bool RollbackSnapshot( object snapshotId, string viewName, Parameters parameters )
		{
			Contract.Requires<ArgumentNullException>( snapshotId != null );

			return false;
		}
		#endregion

		#region Implementation of INavigationService
		public abstract void Navigate( string viewName );
		public abstract void Navigate( string viewName, Parameters parameters );
		public abstract void GoBack();
		#endregion
	}
}