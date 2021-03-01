using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Models.Helpers;

namespace Models
{
    public class FoodType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Food Type Name")]
        [ModelBinder(typeof(TrimmingModelBinder))]
        public string Name { get; set; }
    }
}