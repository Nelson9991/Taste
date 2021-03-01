using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;

namespace Taste.Pages.Admin.FoodType
{
    [Authorize(Roles = Sd.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty] public Models.FoodType FoodType { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            FoodType = new Models.FoodType();
            if (id is not null)
            {
                FoodType = await _unitOfWork.FoodTypeRepository.GetFirstOrDefaultAsync(x => x.Id == id);

                if (FoodType is null)
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

            if (FoodType.Id == 0)
            {
                await _unitOfWork.FoodTypeRepository.AddAsync(FoodType);
            }
            else
            {
                await _unitOfWork.FoodTypeRepository.UpdateAsync(FoodType);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToPage("Index");
        }
    }
}