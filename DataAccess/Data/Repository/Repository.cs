using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            var query = _dbSet.AsQueryable();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            // Include properties will be comma separated
            if (includeProperties is not null)
            {
                foreach (var prop in includeProperties.Split(','))
                {
                    query = query.Include(prop);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            string includeProperties = null)
        {
            var query = _dbSet.AsQueryable();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            // Include properties will be comma separated
            if (includeProperties is not null)
            {
                query = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public static bool IsValidProperty(
            string propertyName,
            bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance);
            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException(
                    String.Format(
                        "ERROR: Property '{0}' does not exist.",
                        propertyName)
                );
            return prop != null;
        }

        public async Task Remove(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            Remove(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}