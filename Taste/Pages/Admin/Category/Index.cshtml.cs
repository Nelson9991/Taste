using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;

namespace Taste.Pages.Admin.Category
{
    [Authorize(Roles = Sd.ManagerRole)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
