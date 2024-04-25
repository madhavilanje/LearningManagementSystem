using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.SqlClietInterfaces
{
    public interface IUserDbContext
    {
        /// <summary>
        ///  Set the Db set.
        /// </summary>
        /// <typeparam name="T"> Class type.</typeparam>
        /// <returns> dB set of the type class. </returns>
        DbSet<T> Set<T>()
            where T : class;

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the asynchronous save operation.
        /// The task result contains the number of state entities written to database.
        /// </returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        ///  Entry the EntityEntry.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T"> Class type.</typeparam>
        /// <returns> EntityEntry of the type class. </returns>
        EntityEntry<T> Entry<T>(T entity)
            where T : class;
    }
}
