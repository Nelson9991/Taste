using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository;
using DataAccess.Data.Repository.IRepository;
using Models;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new JsonResult(new {data = await _unitOfWork.ApplicationUserRepository.GetAllAsync()});
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlockAsync([FromBody] string id)
        {
            var appUser = await _unitOfWork.ApplicationUserRepository.GetFirstOrDefaultAsync(x => x.Id == id);

            if (appUser is null)
            {
                return new JsonResult(new {success = false, message = "Error while locking/unlocking"});
            }

            if (appUser.LockoutEnd is not null && appUser.LockoutEnd > DateTime.Now)
            {
                appUser.LockoutEnd = DateTimeOffset.Now;
            }
            else
            {
                appUser.LockoutEnd = DateTimeOffset.Now.AddYears(100);
            }

            await _unitOfWork.SaveAsync();
            return new JsonResult(new {success = true, message = "Operation Successful!"});
        }
    }
}
