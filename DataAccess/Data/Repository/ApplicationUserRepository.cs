using DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(DbContext context) : base(context)
        {
        }
    }
}