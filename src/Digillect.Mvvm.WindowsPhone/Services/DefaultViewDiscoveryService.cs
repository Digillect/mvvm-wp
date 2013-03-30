﻿#region Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis® Wunderman)
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
		///     Gets the view types in this application.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetViewTypes()
		{
			return _viewTypes;
		}

		/// <summary>
		///     Gets the root namespace that should be ignored when building paths to XAML pages.
		/// </summary>
		/// <returns></returns>
		public string GetRootNamespace()
		{
			return _rootNamespace;
		}
		#endregion
	}
}