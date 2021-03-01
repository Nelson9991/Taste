using System.Threading.Tasks;
using Models;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        public Task UpdateAsync(MenuItem menuItem);
    }
}