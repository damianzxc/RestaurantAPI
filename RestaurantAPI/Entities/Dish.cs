namespace RestaurantAPI.Entities
{
    public class Dish
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // References
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}
