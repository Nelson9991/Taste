using System.Threading.Tasks;
using Models;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        public Task UpdateAsync(OrderDetails orderDetails);
    }
}