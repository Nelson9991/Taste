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
using Stripe;
using Utilities;

namespace Taste.Pages.Admin.Order
{
    public class ManageOrderModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManageOrderModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty] public List<OrderDetailsVM> OrderDetailsVm { get; set; }

        public async Task OnGet(string status = null)
        {
            OrderDetailsVm = new List<OrderDetailsVM>();

            var orderHeaderList = await _unitOfWork.OrderHeaderRepository
                .GetAllAsync(x =>
                        x.Status == Sd.StatusSubmitted || x.Status == Sd.StatusInProcess,
                    x => x.OrderByDescending(y => y.PickUpTime));

            foreach (var orderHeader in orderHeaderList)
            {
                var individual = new OrderDetailsVM()
                {
                    OrderHeader = orderHeader,
                    OrderDetailsList =
                        await _unitOfWork.OrderDetailsRepository.GetAllAsync(x => x.OrderId == orderHeader.Id)
                };

                OrderDetailsVm.Add(individual);
            }
        }

        public async Task<IActionResult> OnPostOrderPrepare(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == orderId);
            orderHeader.Status = Sd.StatusInProcess;
            await _unitOfWork.SaveAsync();
            return RedirectToPage("ManageOrder");
        }

        public async Task<IActionResult> OnPostOrderReady(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == orderId);
            orderHeader.Status = Sd.StatusReady;
            await _unitOfWork.SaveAsync();
            return RedirectToPage("ManageOrder");
        }

        public async Task<IActionResult> OnPostOrderCancel(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(x => x.Id == orderId);
            orderHeader.Status = Sd.StatusCancelled;
            await _unitOfWork.SaveAsync();
            return RedirectToPage("ManageOrder");
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
            return RedirectToPage("ManageOrder");
        }
    }
}
