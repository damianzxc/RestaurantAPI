using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        void DeleteById(int id);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto? GetById(int id);
        void UpdateById(int id, UpdateRestaurantDto dto);
    }
}