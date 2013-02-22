using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Digillect.Mvvm.UI
{
	/// <summary>
	/// Provides infrastructure for page backed up with <see cref="Digillect.Mvvm.EntityViewModel{TId,TModel}"/>.
	/// </summary>
	/// <typeparam name="TId">The type of the entity identifier.</typeparam>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <typeparam name="TViewModel">The type of the page model.</typeparam>
	/// <remarks>Instance of this class performs lookup of the query string upon navigation to find and extract parameter with
	/// name <c>Id</c> that is used as entity id for page model. If that parameter is not found then <see cref="System.ArgumentException"/> will be thrown.</remarks>
	public class EntityPage<TId, TEntity, TViewModel> : ViewModelPage<TViewModel>
		where TId: IComparable<TId>, IEquatable<TId>
		where TEntity: class, IXIdentified<TId>
		where TViewModel: EntityViewModel<TId, TEntity>
	{
		/// <summary>
		/// Loads the data.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns></returns>
		protected override Task<Session> LoadData( DataLoadReason reason )
		{
			return ViewModel.Load( ViewParameters.Get<TId>( "Id" ) );
		}

		/// <summary>
		/// Parses the parameters.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <exception cref="System.ArgumentException">Entity identifier is not passed in query string.</exception>
		protected override void ParseParameters( IDictionary<string, string> queryString )
		{
			base.ParseParameters( queryString );

			string stringId = null;

			if( queryString.TryGetValue( "Id", out stringId ) )
			{
				ViewParameters.Add( "Id", (TId) Services.NavigationService.DecodeValue( stringId, typeof( TId ) ) );

				queryString.Remove( "Id" );
			}
			else
			{
				throw new ArgumentException( "Entity identifier is not passed in query string.", "Id" );
			}
		}
	}
}
