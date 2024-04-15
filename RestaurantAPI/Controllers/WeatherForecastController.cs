using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //private readonly WeatherForecastService _service;   // Strongly related dependency
        //Without strongly related (Dependency Inversion) -> using interfaces
        private readonly IWeatherForecastService _service;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;

        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return _service.Get();
        }

        [HttpGet]
        [Route("currentDay/{max}")]  // or[HttpGet("currentDay")]
        public IEnumerable<WeatherForecast> Get2([FromQuery] int take, [FromRoute] int max)
        {
            return _service.Get();
        }

        [HttpPost]
        public ActionResult<string> Hello([FromBody] string msg)
        {
            //Response.StatusCode = 401;    Won't work
            //return $"{msg}";
            return StatusCode(401, msg);

            // Other option
            //return NotFound(msg);
        }

        // First excercise
        [HttpGet("generate/{maximum}")]
        public ActionResult<IEnumerable<WeatherForecast>> Get([FromBody]int count, [FromQuery]int minimum, [FromRoute]int maximum)
        {
            var weather = _service.GetWithParams(count, minimum, maximum);
            if (weather != null)
            {
                return StatusCode(200, weather);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("generate2")]
        public ActionResult<IEnumerable<WeatherForecast>> Get([FromBody] WeatherRequestDTO dto)
        { 
            var weather = _service.GetWithParams(dto.Count, dto.Minimum, dto.Maximum);
            if (weather != null)
            {
                return StatusCode(200, weather);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
