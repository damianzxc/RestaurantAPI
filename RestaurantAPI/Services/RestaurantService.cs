using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)    // To add Address and Dishes table
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.ID == id) ?? throw new NotFoundException("Restaurant not found");
            ;

            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhrase == null
                    || r.Name.ToUpper().Contains(query.SearchPhrase.ToUpper())
                    || r.Description.ToUpper().Contains(query.SearchPhrase.ToUpper()));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsDictionary = new Dictionary<string, Expression<Func<Restaurant, object?>>>
                {
                    { nameof(Restaurant.Name), r => r.Name },
                    { nameof(Restaurant.Description), r => r.Description },
                    { nameof(Restaurant.Category), r => r.Category }
                };

                var keySelector = columnsDictionary.First(k => k.Key.Equals(query.SortBy)).Value;

                baseQuery = query.SortDirection is SortDirection.ASCENDING ? baseQuery.OrderBy(keySelector)
                    : baseQuery.OrderByDescending(keySelector);
            }

            var restaurants = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var restaurantsCount = baseQuery.Count();

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDto, restaurantsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.ID;
        }

        public void DeleteById(int id)
        {
            _logger.LogError("Restaurant with id: {id} DELETE action invoked", id);

            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.ID == id) ?? throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                new ResourcesOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public void UpdateById(int id, UpdateRestaurantDto dto)
        {

            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(r => r.ID == id) ?? throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, 
                new ResourcesOperationRequirement(ResourceOperation.Update)).Result;

            if(!authorizationResult.Succeeded) 
            {
                throw new ForbidException();
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            _dbContext.SaveChanges();
        }
    }
}
