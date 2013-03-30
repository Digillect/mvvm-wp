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

using System;
using System.Collections.Generic;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	///     Provides infrastructure for page backed up with <see cref="Digillect.Mvvm.EntityViewModel{TModel}" />.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TViewModel">The type of the page model.</typeparam>
	/// <remarks>
	///     Instance of this class performs lookup of the query string upon navigation to find and extract parameter with
	///     name <c>Id</c> that is used as entity id for page model. If that parameter is not found then
	///     <see
	///         cref="System.ArgumentException" />
	///     will be thrown.
	/// </remarks>
	public class EntityPage<TEntity, TViewModel> : ViewModelPage<TViewModel>
		where TEntity : XObject
		where TViewModel : EntityViewModel<TEntity>
	{
		/// <summary>
		///     This method is called to create data loading session.
		/// </summary>
		/// <param name="reason">The reason to load page data.</param>
		/// <returns>Session that should be used to load page data.</returns>
		protected override Session CreateDataSession( DataLoadReason reason )
		{
			return ViewModel.CreateSession( ViewParameters.Get<XKey>( "Key" ) );
		}

		/// <summary>
		///     Parses the parameters.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <exception cref="System.ArgumentException">Entity identifier is not passed in query string.</exception>
		protected override void ParseParameters( IDictionary<string, string> queryString )
		{
			base.ParseParameters( queryString );

			string stringKey;

			if( queryString.TryGetValue( "Key", out stringKey ) )
			{
				if( !string.IsNullOrEmpty( stringKey ) )
				{
					ViewParameters.Add( "Key", XKeySerializer.Deserialize( stringKey ) );
				}

				queryString.Remove( "Key" );
			}
			else
			{
				throw new ArgumentException( "Entity key is not passed in query string.", "Key" );
			}
		}
	}
}