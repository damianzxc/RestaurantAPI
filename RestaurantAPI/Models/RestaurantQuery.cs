using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace RestaurantAPI.Models
{
    public class RestaurantQuery
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<SortDirection>))]
        public SortDirection SortDirection { get; set; }
    }
}
