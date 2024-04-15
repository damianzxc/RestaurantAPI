using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RestaurantAPI.Entities
{
    public class Restaurant
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery {  get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
    
        // References
        public int AddressID { get; set; }
        public virtual Address Address { get; set; }
        public virtual List<Dish> Dishes { get; set; }
    }
}
