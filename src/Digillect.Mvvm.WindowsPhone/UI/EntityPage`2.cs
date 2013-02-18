using System;
using System.Collections.Generic;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Provides infrastructure for page backed up with <see cref="Digillect.Mvvm.EntityViewModel{TId,TModel}"/>.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	/// <remarks>Instance of this class performs lookup of the query string upon navigation to find and extract parameter with
	/// name <c>Id</c> that is used as entity id for view model. If that parameter is not found then <see cref="System.ArgumentException"/> will be thrown.</remarks>
	public class EntityPage<TEntity, TViewModel> : ViewModelPage<TViewModel>
		where TEntity: XObject
		where TViewModel: EntityViewModel<TEntity>
	{
		#region InitialLoadData
		/// <summary>
		/// Initials the load data.
		/// </summary>
		protected override void InitialLoadData()
		{
			ViewModel.Load( ViewParameters.Get<XKey>( "Key" ) );
		}
		#endregion

		/// <summary>
		/// Parses the parameters.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <exception cref="System.ArgumentException">Entity identifier is not passed in query string.</exception>
		protected override void ParseParameters( IDictionary<string, string> queryString )
		{
			base.ParseParameters( queryString );

			string stringKey = null;

			if( queryString.TryGetValue( "Key", out stringKey ) )
			{
				ViewParameters.Add( "Key", XKeySerializer.Deserialize( stringKey ) );

				queryString.Remove( "Key" );
			}
			else
			{
				throw new ArgumentException( "Entity key is not passed in query string.", "Key" );
			}
		}
	}
}
