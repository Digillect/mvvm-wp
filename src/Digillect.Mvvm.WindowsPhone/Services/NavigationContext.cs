#region Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis� Wunderman)
// Copyright (c) 2011-2013 Gregory Nickonov and Andrew Nefedkin (Actis� Wunderman).
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

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Context that describes navigation operation.
	/// </summary>
	public class NavigationContext
	{
		#region Public Properties
		/// <summary>
		///     Gets or sets the name of the view.
		/// </summary>
		/// <value>
		///     The name of the view.
		/// </value>
		public string ViewName { get; set; }

		/// <summary>
		///     Gets or sets view parameters.
		/// </summary>
		/// <value>
		///     The parameters.
		/// </value>
		public XParameters Parameters { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the navigation operation with this context should be cancelled.
		/// </summary>
		/// <value>
		///     <c>true</c> to cancel navigation; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the currently presented view should be removed from navigation journal.
		/// </summary>
		/// <value>
		///     <c>true</c> to displace current view; otherwise, <c>false</c>.
		/// </value>
		public bool DisplaceCurrentView { get; set; }
		#endregion
	}
}