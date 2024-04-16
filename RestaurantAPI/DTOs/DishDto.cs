namespace RestaurantAPI.DTOs
{
    public class DishDto
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
}
