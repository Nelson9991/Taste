using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            var menuItemDb = await _context.MenuItems
                .FirstOrDefaultAsync(x => x.Id == menuItem.Id);

            menuItemDb.Description = menuItem.Description;
            menuItemDb.Name = menuItem.Name;
            menuItemDb.FoodTypeId = menuItem.FoodTypeId;
            menuItemDb.CategoryId = menuItem.CategoryId;
            menuItemDb.Price = menuItem.Price;
            if (!string.IsNullOrEmpty(menuItem.Image))
            {
                menuItemDb.Image = menuItem.Image;
            }

            await _context.SaveChangesAsync();
        }
    }
}