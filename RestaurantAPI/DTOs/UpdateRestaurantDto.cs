﻿namespace RestaurantAPI.DTOs
{
    public class UpdateRestaurantDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool HasDelivery { get; set; }
    }
}
