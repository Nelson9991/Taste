using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price should be grater than $1")]
        public double Price { get; set; }

        [Display(Name = "Category Type")] public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Display(Name = "Food Type")] public int FoodTypeId { get; set; }
        public virtual FoodType FoodType { get; set; }
    }
}