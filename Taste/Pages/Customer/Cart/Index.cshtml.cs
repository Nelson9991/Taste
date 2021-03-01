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
using Models.ViewModels;
using Utilities;

namespace Taste.Pages.Customer.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailsCartVM OrderDetailsCartVM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetInt32(Sd.ShoppingCart) is null ||
                HttpContext.Session.GetInt32(Sd.ShoppingCart) == 0)
            {
                return RedirectToPage("../Home/Index");
            }

            OrderDetailsCartVM = new OrderDetailsCartVM
            {
                OrderHeader = new Models.OrderHeader(),
                CartList = new List<ShoppingCart>()
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claim = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                IEnumerable<ShoppingCart> cart =
                    await _unitOfWork.ShoppingCartRepository.GetAllAsync(x => x.ApplicationUserId == claim.Value);

                if (cart != null)
                {
                    OrderDetailsCartVM.CartList = cart.ToList();
                }

                foreach (var cartItem in OrderDetailsCartVM.CartList)
                {
                    cartItem.MenuItem =
                        await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(x => x.Id == cartItem.MenuItemId);
                    OrderDetailsCartVM.OrderHeader.OrderTotal += (cartItem.MenuItem.Price * cartItem.Count);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostPlus(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.IncrementCount(cart, 1);
            await _unitOfWork.SaveAsync();
            return RedirectToPage("/Customer/Cart/Index");
        }

        public async Task<IActionResult> OnPostMinus(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(x => x.Id == cartId);
            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cart);
                await _unitOfWork.SaveAsync();

                var carts = await _unitOfWork.ShoppingCartRepository.GetAllAsync(x => x.ApplicationUserId == cart.ApplicationUserId);
                var count = carts.Count;

                HttpContext.Session.SetInt32(Sd.ShoppingCart, count);
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.DecrementCount(cart, 1);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToPage("/Customer/Cart/Index");
        }

        public async Task<IActionResult> OnPostRemove(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.Remove(cart);
            await _unitOfWork.SaveAsync();

            var carts = await _unitOfWork.ShoppingCartRepository.GetAllAsync(x => x.ApplicationUserId == cart.ApplicationUserId);
            var count = carts.Count;

            HttpContext.Session.SetInt32(Sd.ShoppingCart, count);
            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}
