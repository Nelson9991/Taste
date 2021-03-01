using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return new JsonResult(new
                {data = await _unitOfWork.MenuItemRepository.GetAllAsync(null, null, "Category,FoodType")});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var foodType = await _unitOfWork.MenuItemRepository.GetFirstOrDefaultAsync(x => x.Id == id);

                if (foodType is null)
                {
                    return new JsonResult(new {success = false, message = "Error while deleting"});
                }

                var imagePath = Path.Combine(_env.WebRootPath, foodType.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _unitOfWork.MenuItemRepository.Remove(foodType);
                await _unitOfWork.SaveAsync();

                return new JsonResult(new {success = true, message = "Delete successful!"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new JsonResult(new {success = false, message = "Error while deleting"});
            }
        }
    }
}