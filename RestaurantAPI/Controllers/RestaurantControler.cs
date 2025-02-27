﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurants")]
    [ApiController]  // Auto Validate instead of => ( ModelState.IsValid)
    [Authorize]
    public class RestaurantControler : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        public RestaurantControler(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<PagedResult<RestaurantDto>> GetAll([FromQuery]RestaurantQuery query)
        {
            var restaurantsDto = _restaurantService.GetAll(query);
            return Ok(restaurantsDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AtLeast20")]
        public ActionResult<Restaurant> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);
            return Ok(restaurant);
        }

        [HttpPost("create")]
        public ActionResult Post([FromBody] CreateRestaurantDto dto)
        {
            var id = _restaurantService.Create(dto);
            return Created($"api/restaurants/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.DeleteById(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateRestaurantDto dto)
        {
            _restaurantService.UpdateById(id, dto);
            return Ok();
        }
    }
}
