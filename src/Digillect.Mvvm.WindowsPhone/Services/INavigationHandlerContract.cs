using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Digillect.Mvvm.Services
{
	[ContractClassFor( typeof( INavigationHandler ) )]
	internal abstract class INavigationHandlerContract : INavigationHandler
	{
		#region Implementation of INavigationHandler
		public Task<bool> HandleNavigation( NavigationContext context )
		{
			Contract.Requires<ArgumentNullException>( context != null );

			return null;
		}
		#endregion
	}
}