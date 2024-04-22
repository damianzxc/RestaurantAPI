using NLog.Web;
using RestaurantAPI.AutoMapper;
using RestaurantAPI.Data;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


// Register service
// First option AddScoped<>() -> Each request will be a new service instance
// Second option AddSingleton<>() -> Will create only single instance of service
// Third option AddTransient<>() -> Will create instance at every Controller call method

// Add DbContext
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(typeof(RestaurantMappingProfile));
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Swagger
builder.Services.AddSwaggerGen();

// Logger Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMeasure>();

// Add NLog
builder.Host.UseNLog();

var app = builder.Build();

// Logger Middleware Registration (before UseHttpRedirection!)
//app.UseErrorHandlingMiddleware(); (=> see Example with static class)
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRequestTimeMeasure();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Use Swagger (before ruting and after usehttpredirection? => not tested)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");   // Default link url
});


app.UseRouting();

// Authorization (must be this place)
app.UseAuthorization();

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