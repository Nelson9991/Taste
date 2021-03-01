using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.ViewModels;
using Stripe;
using Utilities;

namespace Taste.Pages.Admin.Order
{
    public class OrderDetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty] public OrderDetailsVM OrderDetailsVm { get; set; }

        public async Task OnGet(int id)
        {
            OrderDetailsVm = new OrderDetailsVM()
            {
                OrderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == id),
                OrderDetailsList = await _unitOfWork.OrderDetailsRepository.GetAllAsync(x => x.OrderId == id)
            };

            OrderDetailsVm.OrderHeader.ApplicationUser =
                await _unitOfWork.ApplicationUserRepository.GetFirstOrDefaultAsync(x =>
                    x.Id == OrderDetailsVm.OrderHeader.UserId);
        }

        public async Task<IActionResult> OnPostOrderConfirm(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == orderId);
            orderHeader.Status = Sd.StatusCompleted;
            await _unitOfWork.SaveAsync();
            return RedirectToPage("OrderList");
        }

        public async Task<IActionResult> OnPostOrderCancel(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == orderId);
            orderHeader.Status = Sd.StatusCancelled;
            await _unitOfWork.SaveAsync();
            return RedirectToPage("OrderList");
        }

        public async Task<IActionResult> OnPostOrderRefund(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == orderId);
            // refund the amount
            var options = new RefundCreateOptions
            {
                Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                Reason = RefundReasons.RequestedByCustomer,
                Charge = orderHeader.TransactionId,
            };
            var service = new RefundService();
            var refund = service.Create(options);

            orderHeader.Status = Sd.StatusRefunded;
            await _unitOfWork.SaveAsync();
            return RedirectToPage("OrderList");
        }
    }
}
