using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.SqlClietImplementations
{
    /// <summary>
    /// GenericRepository class.
    /// </summary>
    /// <typeparam name="TEntity">entity type.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
         where TEntity : class
    {
        private readonly IUserDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GenericRepository(IUserDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the list of entities after specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>List of entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, int? limit = null, int? offset = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = this.context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (offset.HasValue)
            {
                query = query.Skip(offset.Value);
            }

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Find Async.
        /// </summary>
        /// <param name="filter">filter.</param>
        /// <returns>entity.</returns>
        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await context.Set<TEntity>().FirstOrDefaultAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>models.</returns>
        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            DbSet<TEntity> databaseSet = context.Set<TEntity>();
            return await (filter == null ? databaseSet.AsNoTracking().ToListAsync().ConfigureAwait(false) : databaseSet.AsNoTracking().Where(filter).ToListAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <typeparam name="TResult">Output shape.</typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="selector">The Selector.</param>
        /// <returns>models.</returns>
        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector)
        {
            DbSet<TEntity> databaseSet = context.Set<TEntity>();
            return await (filter == null ? databaseSet.AsNoTracking().Select(selector).ToListAsync().ConfigureAwait(false) : databaseSet.AsNoTracking().Where(filter).Select(selector).ToListAsync().ConfigureAwait(false));
        }

        /// <summary>Add the specified entity.</summary>
        /// <param name="entity">The entity.</param>
        /// <param name="includeProps">includeProps.</param>
        /// <returns>entity.</returns>
        public async Task<TEntity> AddAsync(TEntity entity, params string[] includeProps)
        {
            var trackingEnity = await context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            foreach (string propName in includeProps)
            {
                await trackingEnity.Reference(propName).LoadAsync().ConfigureAwait(false);
            }
            return trackingEnity.Entity;
        }

        /// <summary>
        /// Asynchronously saves all changes to the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the asynchronous save operation.
        /// The task result contains the number of state entities written to database.
        /// </returns>
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>Update the specified entity.</summary>
        /// <param name="entity">The entity.</param>
        /// <returns>entity.</returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var trackingEntity = await context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            context.Entry(entity).State = EntityState.Modified;

            return trackingEntity.Entity;
        }

        /// <summary>
        /// Delete the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<int> DeleteAsync(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
            return await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
