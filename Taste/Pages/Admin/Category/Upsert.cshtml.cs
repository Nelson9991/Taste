using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;

namespace Taste.Pages.Admin.Category
{
    [Authorize(Roles = Sd.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty] public Models.Category Category { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            Category = new Models.Category();
            if (id is not null)
            {
                Category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(x => x.Id == id);

                if (Category == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Category.Id == 0)
            {
                await _unitOfWork.CategoryRepository.AddAsync(Category);
            }
            else
            {
                await _unitOfWork.CategoryRepository.UpdateAsync(Category);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToPage("Index");
        }
    }
}