using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<WeatherForecast> Get2([FromQuery]int take, [FromRoute]int max)
        {
            return _service.Get();
        }

        [HttpPost]
        public ActionResult<string> Hello([FromBody]string msg) 
        {
            //Response.StatusCode = 401;    Won't work
            //return $"{msg}";
            return StatusCode(401, msg);
            
            // Other option
            //return NotFound(msg);
        }
    }
}
