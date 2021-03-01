using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.ViewModels;
using Utilities;

namespace Taste.Pages.Admin.MenuItem
{
    [Authorize(Roles = Sd.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public UpsertModel(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        [BindProperty] public MenuItemViewModel MenuItemVM { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            MenuItemVM = new MenuItemViewModel()
            {
                CategoryList = await _unitOfWork.CategoryRepository.GetCategoryListForDropdownAsync(),
                FoodTypeList = await _unitOfWork.FoodTypeRepository.GetFoodTypeListForDropdownAsync(),
                MenuItem = new Models.MenuItem()
            };

            if (id is not null)
            {
                MenuItemVM.MenuItem = await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(x => x.Id == id);

                if (MenuItemVM == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var webRootPath = _env.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            string fileName;
            string extension;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (MenuItemVM.MenuItem.Id == 0)
            {
                (fileName, extension) = await UploadImageAsyc(webRootPath, files);
                MenuItemVM.MenuItem.Image = @"\images\menuItems\" + fileName + extension;

                await _unitOfWork.MenuItemRepository.AddAsync(MenuItemVM.MenuItem);
            }
            else
            {
                // Edit a Menu Item
                var menuItemDb =
                    await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(x => x.Id == MenuItemVM.MenuItem.Id);

                if (files.Count > 0)
                {
                    (fileName, extension) = await UploadImageAsyc(webRootPath, files, true,
                        menuItemDb.Image);
                    MenuItemVM.MenuItem.Image = @"\images\menuItems\" + fileName + extension;
                }
                else
                {
                    MenuItemVM.MenuItem.Image = menuItemDb.Image;
                }

                await _unitOfWork.MenuItemRepository.UpdateAsync(MenuItemVM.MenuItem);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToPage("Index");
        }

        private async Task<(string, string)> UploadImageAsyc(string webRootPath, IFormFileCollection files,
            bool isEdit = false,
            string imageToDeletePath = null)
        {
            var fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(webRootPath, @"images\menuItems");
            var extension = Path.GetExtension(files[0].FileName);

            if (isEdit && !string.IsNullOrEmpty(imageToDeletePath))
            {
                var deletePath = Path.Combine(webRootPath, imageToDeletePath.TrimStart('\\'));

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
            }

            await using var fs = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create,
                FileAccess.Write);
            await files[0].CopyToAsync(fs);

            return (fileName, extension);
        }
    }
}