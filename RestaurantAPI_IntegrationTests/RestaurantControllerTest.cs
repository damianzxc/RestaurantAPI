using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Data.Common;
using System.Text;

namespace RestaurantAPI_IntegrationTests
{
    public class RestaurantControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public RestaurantControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            { 
                AllowAutoRedirect = false,
            });
        }

        private void SeedRestaurant(Restaurant restaurant)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
        }

        [Theory]
        [MemberData(nameof(ValidQueryData))]
        public async Task GetAllRestaurants_WithRestaurantQuery_ReturnsOkResult(string query)
        {
            // Act
            var response = await _httpClient.GetAsync("/api/restaurants?" + query); // It's not good practice to pass body parameter with GET

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        public static IEnumerable<object[]> ValidQueryData
        {
            get
            {
                yield return new object[] { "PageNumber=1&PageSize=5&SortBy=Category" };
                yield return new object[] { "PageNumber=2&PageSize=10&SortBy=Category" };
                yield return new object[] { "PageNumber=1&PageSize=15&SortBy=Category" };
                yield return new object[] { "PageNumber=1&PageSize=5&SortBy=Name" };
                yield return new object[] { "PageNumber=1&PageSize=5&SortBy=Description" };
            }
        }
    }
}
