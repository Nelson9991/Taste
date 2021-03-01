using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Full Name")] public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped] public string FullName => FirstName + " " + LastName;
    }
}