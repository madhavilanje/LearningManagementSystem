using System.Linq.Expressions;


namespace LMS.Infrastructure.SqlClietInterfaces
{
    /// <summary>
    /// IGenericRepository.
    /// </summary>
    /// <typeparam name="TEntity"> genric entity.</typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {

#nullable enable

        /// <summary>
        /// Gets the list of entities after specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>List of entities.</returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, int? limit = null, int? offset = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "");

        /// <summary>
        /// Find Async.
        /// </summary>
        /// <param name="filter">filter.</param>
        /// <returns>entity.</returns>
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>models.</returns>
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? filter);

#nullable disable

        /// <summary>Add the specified entity.</summary>
        /// <param name="entity">The entity.</param>
        /// <param name="includeProps">includeProps.</param>
        /// <returns>entity.</returns>
        Task<TEntity> AddAsync(TEntity entity, params string[] includeProps);

        /// <summary>
        /// Asynchronously saves all changes to the database.
        /// </summary>
        /// A <see cref="Task{TResult}" /> that represents the asynchronous save operation.
        /// The task result contains the number of state entities written to database.
        /// <returns>Number of saved entity.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>Update the specified entity.</summary>
        /// <param name="entity">The entity.</param>
        /// <returns>entity.</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Delete the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int> DeleteAsync(TEntity entity);
    }
}
