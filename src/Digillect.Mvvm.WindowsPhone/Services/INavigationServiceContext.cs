using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	/// Service that supports navigation between views
	/// </summary>
	public interface INavigationServiceContext
	{
		/// <summary>
		/// Perform navigation to the specified URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		void Navigate( Uri uri );

		/// <summary>
		/// Performs navigation to the previous page.
		/// </summary>
		void GoBack();
		/// <summary>
		/// Performs navigation to the previous page named <paramref name="viewName"/>, if any.
		/// </summary>
		/// <param name="viewName">Name of the view to return to.</param>
		void GoBack( string viewName );

		/// <summary>
		/// Gets the main assembly containing views.
		/// </summary>
		/// <returns></returns>
		Assembly GetMainAssemblyContainingViews();
		/// <summary>
		/// Gets the root namespace of assembly.
		/// </summary>
		/// <returns></returns>
		string GetRootNamespace();
	}
}
