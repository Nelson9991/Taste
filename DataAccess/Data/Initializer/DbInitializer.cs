using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Utilities;

namespace DataAccess.Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                // ReSharper disable once MethodHasAsyncOverload
                if (_context.Database.GetPendingMigrations().Any())
                {
                    // ReSharper disable once MethodHasAsyncOverload
                    _context.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (_context.Roles.Any(x => x.Name == Sd.ManagerRole))
            {
                return;
            }

            _roleManager.CreateAsync(new IdentityRole(Sd.ManagerRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Sd.CusomerRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Sd.FrontDeskRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Sd.KitchenRole)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser()
            {
                Email = "admin@gmail.com",
                UserName = "admin@gmail.com",
                EmailConfirmed = true,
                FirstName = "Nelson",
                LastName = "Marro"
            }, "Admin123*").GetAwaiter().GetResult();

            var user = _context.Users.FirstOrDefault(x => x.UserName == "admin@gmail.com");
            _userManager.AddToRoleAsync(user, Sd.ManagerRole).GetAwaiter().GetResult();
        }
    }
}