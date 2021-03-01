using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace DataAccess.Data.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<IList<SelectListItem>> GetCategoryListForDropdownAsync();
        public Task UpdateAsync(Category category);
    }
}