using RestaurantAPI.AutoMapper;
using RestaurantAPI.Data;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register service
// First option AddScoped<>() -> Each request will be a new service instance
// Second option AddSingleton<>() -> Will create only single instance of service
// Third option AddTransient<>() -> Will create instance at every Controller call method
builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();

// Add DbContext
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(typeof(RestaurantMappingProfile));
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});


app.MapControllers();

// Seed Database cont.
SeedDatabase();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
        dbInitializer.Seed();
    }
}
app.Run();