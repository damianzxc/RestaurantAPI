using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;

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
        //[AllowAnonymous]
        [Authorize(Policy = "HasNationality")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantsDto = _restaurantService.GetAll();
            return Ok(restaurantsDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AddLeast20")]
        public ActionResult<Restaurant> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);
            return Ok(restaurant);
        }

        [HttpPost("create")]
        public ActionResult Post([FromBody] CreateRestaurantDto dto)
        {
            // Validation (ex. Restaurant name should have max 25 chars)
            //if (dto.Name.Length > 25)... but better add annotation on CreateRestaurantDTO class
            //if (!ModelState.IsValid)
            //{ 
            //    return BadRequest(ModelState);
            //}
            
            var id = _restaurantService.Create(dto);
            return Created($"api/restaurants/{id}", null);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.DeleteById(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateRestaurantDto dto)
        { 
            _restaurantService.UpdateById(id, dto);
            return Ok();
        }
    }
}
