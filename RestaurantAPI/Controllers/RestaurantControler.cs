﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurants")]
    public class RestaurantControler : ControllerBase
    {
        private readonly RestaurantDbContext _db;
        private readonly IMapper _mapper;

        public RestaurantControler(RestaurantDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> Get()
        {
            var restaurants = _db
                .Restaurants
                .Include(r => r.Address)    // To add Address and Dishes table
                .Include(r => r.Dishes)
                .ToList();

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            return Ok(restaurantsDto);
        }

        [HttpGet("{id}")]
        public ActionResult<Restaurant> Get([FromRoute] int id)
        {
            var restaurant = _db.Restaurants
                .Include(r => r.Address)    // To add Address and Dishes table
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.ID == id);
            if (restaurant is null)
            {
                return NotFound();
            }

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            {
                return Ok(restaurantDto);
            }
        }

        [HttpPost("create")]
        public ActionResult Post([FromBody] CreateRestaurantDto dto)
        {
            // Validation (ex. Restaurant name should have max 25 chars)
            //if (dto.Name.Length > 25)... but better add annotation on CreateRestaurantDTO class
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }
            
            var restaurant = _mapper.Map<Restaurant>(dto);
            _db.Restaurants.Add(restaurant);
            _db.SaveChanges();

            return Created($"api/restaurant/{restaurant.ID}", null);
        }
    }
}
