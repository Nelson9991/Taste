using System;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;

namespace DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository CategoryRepository { get; private set; }
        public IFoodTypeRepository FoodTypeRepository { get; private set; }
        public IMenuItemRepository MenuItemRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }

        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

        public IOrderDetailsRepository OrderDetailsRepository { get; private set; }
        public ISP_Call SpCall { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(context);
            FoodTypeRepository = new FoodTypeRepository(context);
            MenuItemRepository = new MenuItemRepository(context);
            ApplicationUserRepository = new ApplicationUserRepository(context);
            ShoppingCartRepository = new ShoppingCartRepository(context);
            OrderDetailsRepository = new OrderDetailsRepository(context);
            OrderHeaderRepository = new OrderHeaderRepository(context);
            SpCall = new SP_Call(context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_context is not null)
            {
                await _context.DisposeAsync();
            }
        }
    }
}