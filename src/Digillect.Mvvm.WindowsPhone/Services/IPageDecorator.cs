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

using System.Diagnostics.Contracts;

using Digillect.Mvvm.UI;

namespace Digillect.Mvvm.Services
{
	/// <summary>
	///     Interface that is used to perform uniform page decoration.
	/// </summary>
	/// <remarks>
	///     Decoration can be used, for example, to add certain UI elements to all pages
	///     (or to selected subset of pages) without the need to modify markup or code-behind classes.
	/// </remarks>
	[ContractClass( typeof( IPageDecoratorContract ) )]
	public interface IPageDecorator
	{
		/// <summary>
		///     Adds the decoration to the page.
		/// </summary>
		/// <param name="page">The page to decorate.</param>
		void AddDecoration( Page page );

		/// <summary>
		///     Optionally removes the decoration from the page.
		/// </summary>
		/// <param name="page">The page to remove decoration from.</param>
		void RemoveDecoration( Page page );
	}
}