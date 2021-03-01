using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace DataAccess.Data.Repository.IRepository
{
    public interface IFoodTypeRepository : IRepository<FoodType>
    {
        public Task<IList<SelectListItem>> GetFoodTypeListForDropdownAsync();
        public Task UpdateAsync(FoodType foodType);
    }
}