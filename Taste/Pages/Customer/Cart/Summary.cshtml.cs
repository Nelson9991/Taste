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
using Models.ViewModels;
using Stripe;
using Utilities;

namespace Taste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty] public OrderDetailsCartVM DetailsCartVM { get; set; }

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

        public async Task<IActionResult> OnPost(string stripeToken)
        {
            var claim = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);

            DetailsCartVM.CartList =
                await _unitOfWork.ShoppingCartRepository.GetAllAsync(x => x.ApplicationUserId == claim.Value);

            DetailsCartVM.OrderHeader.PaymentStatus = Sd.PaymentStatusPending;
            DetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            DetailsCartVM.OrderHeader.UserId = claim.Value;
            DetailsCartVM.OrderHeader.Status = Sd.PaymentStatusPending;
            DetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(
                DetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + " " +
                DetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new();
            await _unitOfWork.OrderHeaderRepository.AddAsync(DetailsCartVM.OrderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var shoppingCart in DetailsCartVM.CartList)
            {
                shoppingCart.MenuItem =
                    await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(x => x.Id == shoppingCart.MenuItemId);
                OrderDetails orderDetails = new()
                {
                    MenuItemId = shoppingCart.MenuItemId,
                    OrderId = DetailsCartVM.OrderHeader.Id,
                    Description = shoppingCart.MenuItem.Description,
                    Name = shoppingCart.MenuItem.Name,
                    Price = shoppingCart.MenuItem.Price,
                    Count = shoppingCart.Count
                };

                DetailsCartVM.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price);
                await _unitOfWork.OrderDetailsRepository.AddAsync(orderDetails);
            }

            DetailsCartVM.OrderHeader.OrderTotal =
                Convert.ToDouble(DetailsCartVM.OrderHeader.OrderTotal.ToString("0.##"));

            _unitOfWork.ShoppingCartRepository.RemoveRange(DetailsCartVM.CartList);
            HttpContext.Session.SetInt32(Sd.ShoppingCart, 0);
            await _unitOfWork.SaveAsync();

            if (!string.IsNullOrEmpty(stripeToken))
            {
                var options = new ChargeCreateOptions
                {
                    // Amount is in cents
                    Amount = Convert.ToInt32(DetailsCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Source = stripeToken,
                    Description = $"Order ID: {DetailsCartVM.OrderHeader.Id}",
                };
                var service = new ChargeService();
                var charge = service.Create(options);

                DetailsCartVM.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    // email
                    DetailsCartVM.OrderHeader.PaymentStatus = Sd.PaymentStatusApproved;
                    DetailsCartVM.OrderHeader.Status = Sd.StatusSubmitted;
                }
                else
                {
                    DetailsCartVM.OrderHeader.PaymentStatus = Sd.PaymentStatusRejected;
                }
            }
            else
            {
                DetailsCartVM.OrderHeader.PaymentStatus = Sd.PaymentStatusRejected;
            }

            await _unitOfWork.SaveAsync();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new {orderId = DetailsCartVM.OrderHeader.Id});
        }
    }
}
