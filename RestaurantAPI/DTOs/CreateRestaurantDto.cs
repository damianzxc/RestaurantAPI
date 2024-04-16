namespace RestaurantAPI.DTOs
{
    public class CreateRestaurantDto
    {
        // Basic properties because for adding Dishes we will create another DTO
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }

        // Properties to add address
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}
