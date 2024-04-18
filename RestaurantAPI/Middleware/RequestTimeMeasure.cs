using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace RestaurantAPI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestTimeMeasure : IMiddleware
    {
        private Stopwatch _stopwatch;
        private readonly ILogger<RequestTimeMeasure> _logger;

        public RequestTimeMeasure(ILogger<RequestTimeMeasure> logger)
        {
            _stopwatch = new Stopwatch();
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();

            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            if (elapsedMilliseconds / 1000 > 4)
            { 
                _logger.LogInformation($"Request {context.Request.Method} at {context.Request.Path} took {elapsedMilliseconds} ms");
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestTimeMeasureExtensions
    {
        public static IApplicationBuilder UseRequestTimeMeasure(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimeMeasure>();
        }
    }
}
