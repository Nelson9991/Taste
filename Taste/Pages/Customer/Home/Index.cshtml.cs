using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Utilities;

namespace Taste.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MenuItem> MenuItems { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public async Task OnGet()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var lsCarts =
                    await _unitOfWork.ShoppingCartRepository.GetAllAsync(x => x.ApplicationUserId == claim.Value);
                var count = lsCarts.Count;
                HttpContext.Session.SetInt32(Sd.ShoppingCart, count);
            }

            MenuItems = await _unitOfWork.MenuItemRepository.GetAllAsync(null, null, "Category,FoodType");
            Categories =
                await _unitOfWork.CategoryRepository.GetAllAsync(null, q => q.OrderBy(c => c.DisplayOrder), null);
        }
    }
}
