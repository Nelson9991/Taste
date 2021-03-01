using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IList<SelectListItem> CategoryList { get; set; }
        public IList<SelectListItem> FoodTypeList { get; set; }
    }
}