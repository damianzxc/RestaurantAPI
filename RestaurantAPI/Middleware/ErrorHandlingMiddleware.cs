
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException nfe)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(nfe.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }

    // Example of using static class to add Middleware to app
    //public static class ErrorHandlingMiddlewareExtension
    //{ 
    //    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder app)
    //    {
    //        return app.UseMiddleware<ErrorHandlingMiddleware>();
    //    }
    //}
}
