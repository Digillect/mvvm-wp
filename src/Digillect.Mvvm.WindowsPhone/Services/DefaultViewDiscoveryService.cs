using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Provides framework with information about view types based on current Application class.
	/// </summary>
	public class DefaultViewDiscoveryService : IViewDiscoveryService
	{
		private readonly string _rootNamespace;
		private readonly List<Type> _viewTypes;

		#region Constructors/Disposer
		/// <summary>
		///     Initializes a new instance of the <see cref="DefaultViewDiscoveryService" /> class.
		/// </summary>
		public DefaultViewDiscoveryService()
		{
			Application app = Application.Current;
			Type appType = app.GetType();

			_rootNamespace = appType.Namespace;
			_viewTypes = appType.Assembly.GetTypes().Where( t => t.GetCustomAttributes( typeof( ViewAttribute ), false ).Any() ).ToList();
		}
		#endregion

		#region IViewDiscoveryService Members
		/// <summary>
		/// Gets the view types in this application.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetViewTypes()
		{
			return _viewTypes;
		}

		/// <summary>
		/// Gets the root namespace that should be ignored when building paths to XAML pages.
		/// </summary>
		/// <returns></returns>
		public string GetRootNamespace()
		{
			return _rootNamespace;
		}
		#endregion
	}
}