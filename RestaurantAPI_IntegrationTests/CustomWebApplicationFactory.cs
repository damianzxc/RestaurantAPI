using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantAPI_IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
                    services.Remove(dbContextDescriptor);

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                    services.AddMvc(o => o.Filters.Add(new FakeUserFilter()));

                    services.AddDbContext<RestaurantDbContext>((options) =>
                    {
                        options.UseInMemoryDatabase("RestaurantDb");
                    });
            });

            builder.UseEnvironment("Development");
        }
    }
    
}
