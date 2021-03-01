using System.Threading.Tasks;
using Models;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public Task UpdateAsync(OrderHeader orderHeader);
    }
}