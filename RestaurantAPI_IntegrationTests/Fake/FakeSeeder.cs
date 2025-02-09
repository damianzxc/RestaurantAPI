using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Entities;

namespace RestaurantAPI_IntegrationTests.Fake
{
    internal static class FakeSeeder
    {
        internal static void SeedRestaurant(Restaurant restaurant, CustomWebApplicationFactory<Program> factory)
            => Seed<Restaurant>(restaurant, factory);

        internal static void SeedRestaurants(IEnumerable<Restaurant> restaurants, CustomWebApplicationFactory<Program> factory)
        {
            foreach (var item in restaurants)
            {
                Seed<Restaurant>(item, factory);
            }
        }

        internal static void SeedAddress(Address address, CustomWebApplicationFactory<Program> factory)
            => Seed<Address>(address, factory);

        private static void Seed<T>(T value, CustomWebApplicationFactory<Program> factory) where T : class
        {
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            dbContext.Set<T>().Add(value);
            dbContext.SaveChanges();
        }
    }
}
