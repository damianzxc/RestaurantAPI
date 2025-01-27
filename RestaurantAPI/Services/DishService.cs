using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Services
{
    public interface IDishService
    { 
        public int Create(int restaurantId, CreateDishDto dto);
        public DishDto GetById(int restaurantId, int dishId);
        public IEnumerable<DishDto> GetAll(int restaurantId);
        void Delete(int restaurantId, int dishId);
        void DeleteAll(int restaurantId);
    }
    public class DishService(RestaurantDbContext context, IMapper mapper) : IDishService
    {
        private readonly RestaurantDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public int Create(int restaurantId, CreateDishDto dto)
        {
            // otherwise throw exception
            _ = GetRestaurantById(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restaurantId;

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();

            return dishEntity.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            Dish dish = GetDishById(dishId, restaurantId);
            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public IEnumerable<DishDto> GetAll(int restaurantId)
        {
            Restaurant restaurant = GetRestaurantWithDishes(restaurantId);

            var dishDtos = _mapper.Map<IEnumerable<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }

        public void Delete(int restaurantId, int dishId)
        {
            var dish = GetDishById(dishId, restaurantId);

            _context.Dishes.Remove(dish);
            _context.SaveChanges();
        }

        public void DeleteAll(int restaurantId)
        {
            // throw exception if restaurant has no dishes
            var restaurant = GetRestaurantWithDishes(restaurantId);

            _context.RemoveRange(restaurant.Dishes!);
            _context.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == restaurantId)
                ?? throw new NotFoundException("Restaurant not found");
            
            return restaurant;
        }

        private Dish GetDishById(int dishId, int restaurantId)
        {
            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId)
                ?? throw new NotFoundException("Dish not fount");

            if (dish.RestaurantId != restaurantId)
                throw new NotFoundException("Dish not belongs to the restaurant");

            return dish;
        }

        private Restaurant GetRestaurantWithDishes(int restaurantId)
        {
            var restaurant =  _context
                .Restaurants
                .Include(d => d.Dishes)
                .FirstOrDefault(r => r.ID == restaurantId) ?? throw new NotFoundException("Restaurant not found");

            if (restaurant.Dishes?.Count > 0)
                return restaurant;
            else
                throw new NotFoundException("Restaurant has no dishes");

        }
    }
}
