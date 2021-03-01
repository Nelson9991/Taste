using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(OrderHeader orderHeader)
        {
            var orderHeaderDb = await _context.OrderHeaders
                .FirstOrDefaultAsync(x => x.Id == orderHeader.Id);

            _context.OrderHeaders.Update(orderHeaderDb);

            await _context.SaveChangesAsync();
        }
    }
}