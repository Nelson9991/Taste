using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Models;
using Models.ViewModels;
using Utilities;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string status = null)
        {
            try
            {
                List<OrderDetailsVM> orderListVm = new();

                IEnumerable<OrderHeader> orderHeaders;

                if (User.IsInRole(Sd.CusomerRole))
                {
                    // retieve all orders for thah customer
                    var claims = User.FindFirst(ClaimTypes.NameIdentifier);

                    orderHeaders =
                        await _unitOfWork.OrderHeaderRepository.GetAllAsync(x => x.UserId == claims.Value, null,
                            "ApplicationUser");
                }
                else
                {
                    orderHeaders =
                        await _unitOfWork.OrderHeaderRepository.GetAllAsync(null, null,
                            "ApplicationUser");
                }

                if (status == "cancelled")
                {
                    orderHeaders =
                        orderHeaders.Where(x =>
                            x.Status == Sd.StatusCancelled || x.Status == Sd.StatusRefunded ||
                            x.Status == Sd.PaymentStatusRejected);
                }
                else if (status == "completed")
                {
                    orderHeaders =
                        orderHeaders.Where(x => x.Status == Sd.StatusCompleted);
                }
                else
                {
                    orderHeaders =
                        orderHeaders.Where(x =>
                            x.Status == Sd.StatusReady || x.Status == Sd.StatusInProcess ||
                            x.Status == Sd.StatusSubmitted || x.Status == Sd.PaymentStatusPending);
                }

                foreach (var orderHeader in orderHeaders)
                {
                    var individual = new OrderDetailsVM()
                    {
                        OrderHeader = orderHeader,
                        OrderDetailsList =
                            await _unitOfWork.OrderDetailsRepository.GetAllAsync(x => x.OrderId == orderHeader.Id)
                    };

                    orderListVm.Add(individual);
                }

                return new JsonResult(new {data = orderListVm});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                throw;
            }
        }
    }
}
