using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI_IntegrationTests.Fake;
using System.Net.Http.Json;

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

        [Theory]
        [InlineData("3")]
        public async Task GetRestaurantWithCorrectId_ReturnsOkResult(string id)
        {
            // Arrange
            var restaurant = new Restaurant { ID = 3, Name = "Test" };

            // Act
            FakeSeeder.SeedRestaurant(restaurant, _factory);
            var response = await _httpClient.GetAsync($"/api/restaurants/{id}");
            
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRestaurantById_ReturnsRestaurantDto()
        {
            // Arrange
            var address = new Address
            {
                Id = 1,
                City = "Krakow",
                Street = "Krakowska"
            };

            var restaurant = new Restaurant
            {
                ID = 11,
                Name = "Test",
                AddressID = 1
            };
            var testId = 11;

            // Act
            FakeSeeder.SeedAddress(address, _factory);
            FakeSeeder.SeedRestaurant(restaurant, _factory);
            var response = await _httpClient.GetAsync($"api/restaurants/{testId}");
            var restaurantDto = await response.Content.ReadFromJsonAsync<RestaurantDto>();

            // Assert
            restaurantDto.Should().NotBeNull();
            restaurantDto.ID.Should().Be(testId);
            restaurantDto.Name.Should().Be("Test");
            restaurantDto.City.Should().Be("Krakow");
        }


        // TODO: RestaurantService -> Unit test or include Address & Dishes here
        [Theory]
        [InlineData("PageNumber=1&PageSize=5&SortBy=Name&SearchPhrase=SearchPhrase45332")]
        public async Task GetAllRestaurants_WithRestaurantQuery_ReturnsPagedResultOfRestaurantDto(string query)
        {
            // Act
            var restaurants = new List<Restaurant>()
            {
                new() { ID = 71, Name= "Test", Description = "Testing_SearchPhrase45332" },
                new() { ID = 72, Name= "Test", Description = "Testing_SearchPhrase45332" },
                new() { ID = 73, Name= "Test", Description = "Testing_SearchPhrase45332" },
            };
            FakeSeeder.SeedRestaurants(restaurants, _factory);

            // Arrange
            var response = await _httpClient.GetAsync($"/api/restaurants?{query}");

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var responseString = await reader.ReadToEndAsync();
            var pagedResult = JsonConvert.DeserializeObject<PagedResult<RestaurantDto>>(responseString);

            // Assert
            Assert.NotNull(pagedResult);
            Assert.NotNull(pagedResult.Items);
            pagedResult.Items.Count.Should().Be(3);
        }

        [Theory]
        [InlineData("PageNumber=1&PageSize=5&SortBy=Category")]
        [InlineData("PageNumber=1&PageSize=10&SortBy=Category")]
        [InlineData("PageNumber=1&PageSize=15&SortBy=Category")]
        [InlineData("PageNumber=1&PageSize=5&SortBy=Name")]
        [InlineData("PageNumber=1&PageSize=5&SortBy=Description")]
        public async Task GetAllRestaurants_WithRestaurantQuery_ReturnsOkResult(string query)
        {
            // Act
            var response = await _httpClient.GetAsync("/api/restaurants?" + query);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("")]
        [InlineData("PageNumber=2&PageSize=10")]
        [InlineData("PageNumber=1&PageSize=3&SortBy=Category")]
        [InlineData("PageNumber=1&PageSize=54&SortBy=NameDesc")]
        [InlineData("PageNumber=1")]
        public async Task GetAllRestaurants_WithInvalidQueryData_ReturnsBadRequest(string query)
        {
            // Act
            var response = await _httpClient.GetAsync($"/api/restaurants?{query}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
