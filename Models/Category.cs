using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Models.Helpers;

namespace Models
{
    public class Category
    {
        [Key] public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [ModelBinder(typeof(TrimmingModelBinder))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}