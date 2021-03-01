using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.ViewModels;

namespace Taste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailsCartVM DetailsCartVM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            DetailsCartVM = new OrderDetailsCartVM
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailsCartVM.OrderHeader.OrderTotal = 0;

            var claim = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);

            IEnumerable<ShoppingCart> cart = await _unitOfWork.ShoppingCartRepository.GetAllAsync(x => x.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                DetailsCartVM.CartList = cart.ToList();
            }

            foreach (var cartItem in DetailsCartVM.CartList)
            {
                cartItem.MenuItem = await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(x => x.Id == cartItem.MenuItemId);
                DetailsCartVM.OrderHeader.OrderTotal += (cartItem.MenuItem.Price * cartItem.Count);
            }

            ApplicationUser user = await _unitOfWork.ApplicationUserRepository.GetFirstOrDefaultAsync(x => x.Id == claim.Value);
            DetailsCartVM.OrderHeader.PickUpName = user.FirstName;
            DetailsCartVM.OrderHeader.PickUpTime = DateTime.Now;
            DetailsCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            return Page();
        }
    }
}
