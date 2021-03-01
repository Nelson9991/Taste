using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetAsync(int id);

        public Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
        );

        public Task<T> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
        );

        public Task AddAsync(T entity);
        public Task Remove(int id);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
    }
}