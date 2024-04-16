using RestaurantAPI.Entities;

namespace RestaurantAPI.DTOs
{
    public class RestaurantDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool HasDelivery { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public List<DishDto>? Dishes { get; set; }
    }
}
