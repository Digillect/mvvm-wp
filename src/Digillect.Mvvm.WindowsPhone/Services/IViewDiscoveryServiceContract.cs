using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Digillect.Mvvm.Services
{
	[ContractClassFor( typeof( IViewDiscoveryService ) )]
	internal abstract class IViewDiscoveryServiceContract : IViewDiscoveryService
	{
		#region Implementation of IViewDiscoveryService
		public IEnumerable<Type> GetViewTypes()
		{
			Contract.Ensures( Contract.Result<IEnumerable<Type>>() != null );

			return null;
		}

		public string GetRootNamespace()
		{
			Contract.Ensures( Contract.Result<string>() != null );

			return null;
		}
		#endregion
	}
}