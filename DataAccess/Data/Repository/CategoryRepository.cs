using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<SelectListItem>> GetCategoryListForDropdownAsync()
        {
            return await _context.Categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var categoryFromDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

            categoryFromDb.Name = category.Name;
            categoryFromDb.DisplayOrder = category.DisplayOrder;

            await _context.SaveChangesAsync();
        }
    }
}