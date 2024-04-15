
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
        IEnumerable<WeatherForecast>? GetWithParams(int count, int minimum, int maximum);
    }
}