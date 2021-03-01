using System;
using System.Threading.Tasks;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public ICategoryRepository CategoryRepository { get; }
        public IFoodTypeRepository FoodTypeRepository { get; }
        public IMenuItemRepository MenuItemRepository { get; }
        public IApplicationUserRepository ApplicationUserRepository { get; }
        public IShoppingCartRepository ShoppingCartRepository { get; }
        public IOrderHeaderRepository OrderHeaderRepository { get; }
        public IOrderDetailsRepository OrderDetailsRepository { get; }
        public ISP_Call SpCall { get; }
        public Task SaveAsync();
    }
}