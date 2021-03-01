using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(OrderDetails orderDetails)
        {
            var orderDetailsDb = await _context.OrderDetails
                .FirstOrDefaultAsync(x => x.Id == orderDetails.Id);

            _context.OrderDetails.Update(orderDetailsDb);

            await _context.SaveChangesAsync();
        }
    }
}