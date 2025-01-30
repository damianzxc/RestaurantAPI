using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        void DeleteById(int id);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        RestaurantDto? GetById(int id);
        void UpdateById(int id, UpdateRestaurantDto dto);
    }
}