using RestaurantAPI.DTOs;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto, int userId);
        void DeleteById(int id, ClaimsPrincipal user);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto? GetById(int id);
        void UpdateById(int id, UpdateRestaurantDto dto, ClaimsPrincipal user);
    }
}