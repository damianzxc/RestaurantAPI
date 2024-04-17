using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        bool DeleteById(int id);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto? GetById(int id);
        bool UpdateById(int id, UpdateRestaurantDto dto);
    }
}