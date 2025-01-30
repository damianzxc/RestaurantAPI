using RestaurantAPI.DTOs;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        void DeleteById(int id);
        IEnumerable<RestaurantDto> GetAll(string? searchPhrase);
        RestaurantDto? GetById(int id);
        void UpdateById(int id, UpdateRestaurantDto dto);
    }
}