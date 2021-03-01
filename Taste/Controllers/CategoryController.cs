using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Models;
using Utilities;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Sd.ManagerRole)]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return new JsonResult(new {data = await _unitOfWork.CategoryRepository.GetAllAsync()});
            //return new JsonResult(new {data = await _unitOfWork.SpCall.ReturnList<Category>("[dbo].[GetAllCategory]")});
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
            {
                return new JsonResult(new {success = false, message = "Error while deleting"});
            }

            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.SaveAsync();

            return new JsonResult(new {success = true, message = "Delete successful!"});
        }
    }
}