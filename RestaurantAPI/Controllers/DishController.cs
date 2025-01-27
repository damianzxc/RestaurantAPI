using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurants/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _service;
        public DishController(IDishService service) 
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId, [FromBody]CreateDishDto dto)
        {
            var dishId = _service.Create(restaurantId, dto);

            return Created($"/api/restaurants/{restaurantId}/dish{dishId}", null);
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = _service.GetById(restaurantId, dishId);

            return Ok(dish);
        }

        [HttpGet]
        public ActionResult<IEnumerable<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var dishes = _service.GetAll(restaurantId);

            return Ok(dishes);
        }

        [HttpDelete("{dishId}")]
        public ActionResult Delete([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _service.Delete(restaurantId, dishId);
            return Ok("Dish removed");
        }

        [HttpDelete]
        public ActionResult Delete([FromRoute] int restaurantId)
        {
            _service.DeleteAll(restaurantId);
            return Ok();
        }
    }
}
