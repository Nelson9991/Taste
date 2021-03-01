using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data.Repository
{
    public class FoodTypeRepository : Repository<FoodType>, IFoodTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<SelectListItem>> GetFoodTypeListForDropdownAsync()
        {
            return await _context.FoodTypes.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task UpdateAsync(FoodType foodType)
        {
            var foodTypeDb = await _context.FoodTypes.FirstOrDefaultAsync(x => x.Id == foodType.Id);

            foodTypeDb.Name = foodType.Name;
            await _context.SaveChangesAsync();
        }
    }
}