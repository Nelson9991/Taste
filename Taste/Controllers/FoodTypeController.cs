using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FoodTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return new JsonResult(new {data = await _unitOfWork.FoodTypeRepository.GetAllAsync()});
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var foodType = await _unitOfWork.FoodTypeRepository.GetFirstOrDefaultAsync(x => x.Id == id);

            if (foodType is null)
            {
                return new JsonResult(new {success = false, message = "Error while deleting"});
            }

            _unitOfWork.FoodTypeRepository.Remove(foodType);
            await _unitOfWork.SaveAsync();

            return new JsonResult(new {success = true, message = "Delete successful!"});
        }
    }
}