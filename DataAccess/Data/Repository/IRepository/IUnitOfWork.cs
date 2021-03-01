using System;
using System.Threading.Tasks;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public ICategoryRepository CategoryRepository { get; }
        public Task SaveAsync();
    }
}