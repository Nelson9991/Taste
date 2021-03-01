using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Utilities;

namespace Taste.Pages.Customer.Home
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty] public ShoppingCart ShoppingCart { get; set; }

        public async Task OnGet(int id)
        {
            ShoppingCart = new ShoppingCart()
            {
                MenuItem = await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(
                    includeProperties: "Category,FoodType", filter: x => x.Id == id),
                MenuItemId = id
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity) User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCart.ApplicationUserId = claim.Value;

                var cartDb =
                    await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(x =>
                        x.ApplicationUserId == ShoppingCart.ApplicationUserId &&
                        x.MenuItemId == ShoppingCart.MenuItemId);

                if (cartDb is null)
                {
                    await _unitOfWork.ShoppingCartRepository.AddAsync(ShoppingCart);
                }
                else
                {
                    _unitOfWork.ShoppingCartRepository.IncrementCount(cartDb, ShoppingCart.Count);
                }

                await _unitOfWork.SaveAsync();

                var carts = await _unitOfWork.ShoppingCartRepository.GetAllAsync(x =>
                    x.ApplicationUserId == ShoppingCart.ApplicationUserId);
                var count = carts.Count;
                HttpContext.Session.SetInt32(Sd.ShoppingCart, count);
                return RedirectToPage("Index");
            }
            else
            {
                ShoppingCart.MenuItem = await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(
                    includeProperties: "Category,FoodType", filter: x => x.Id == ShoppingCart.MenuItemId);

                return Page();
            }
        }
    }
}
