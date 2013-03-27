using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Provides framework with information about view types.
	/// </summary>
	[ContractClass( typeof( IViewDiscoveryServiceContract ) )]
	public interface IViewDiscoveryService
	{
		/// <summary>
		/// Gets the view types in this application.
		/// </summary>
		IEnumerable<Type> GetViewTypes();
		/// <summary>
		/// Gets the root namespace that should be ignored when building paths to XAML pages.
		/// </summary>
		string GetRootNamespace();
	}
}