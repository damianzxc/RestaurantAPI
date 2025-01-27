using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs
{
    public class CreateDishDto
    {
        [Required]
        public String Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? RestaurantId { get; set; }
    }
}
