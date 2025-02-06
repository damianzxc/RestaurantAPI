using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using RestaurantAPI;
using RestaurantAPI.Authorization;
using RestaurantAPI.AutoMapper;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.DTOs.Validators;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using RestaurantAPI.Services;
using System.Text;

#region Create Web Host
var builder = WebApplication.CreateBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

#endregion

#region Configure Services (DI)

// Authentication Settings
var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;   // Don't enforce HTTPS
    cfg.SaveToken = true;   // Token should be stored on the server side.
    cfg.TokenValidationParameters = new TokenValidationParameters   // Token validation
    {
        ValidIssuer = authenticationSettings.JwtIssuer,     // Token publisher
        ValidAudience = authenticationSettings.JwtIssuer,   // Entities that can use token (within a single app, refer to ourselves)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),     // Private key -> appsettings.json
    };
});

// Own Authorization Policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "France", "Polish"))
    .AddPolicy("AtLeast20", builder => builder.AddRequirements(new MinimumAgeRequired(20)))
    .AddPolicy("CreatedAtLeast2Restaurants", builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));

builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequrementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationResultMiddleware>();

// Add services to the container.
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDto>();    // Register Validators
builder.Services.AddFluentValidationAutoValidation();                       // The same old MVC pipeline behavior
builder.Services.AddFluentValidationClientsideAdapters();                   // For client side

// Add DbContext
builder.Services.AddDbContext<RestaurantDbContext>(options 
    => options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection")));

builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(typeof(RestaurantMappingProfile));
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// FluentValidation - register validators
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();

// Middlewares
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMeasure>();

// UserContextService - User object propagation [Context accessor]
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

// Swagger
builder.Services.AddSwaggerGen();

// CORS
string[]? allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
if (allowedOrigins is not null)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontEndClient", builder =>
        {
            builder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(allowedOrigins);
        });
    });


#endregion

#region Create Middlewares
var app = builder.Build();

// File cashing in browser
app.UseResponseCaching();

// Static files -> app can use files form folder wwwroot
// It's important to add this line at the beginning to not share static files with API
app.UseStaticFiles();

// CORS
app.UseCors("FrontEndClient");

// Logger Middleware Registration (before UseHttpRedirection!)
//app.UseErrorHandlingMiddleware(); (=> see Example with static class)
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRequestTimeMeasure();

// All request have to use authentication. Above UseHttpRecirection!
app.UseAuthentication();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Swagger configuration
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
#endregion

#region Database Seeder
SeedDatabase();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
        dbInitializer.Seed();
    }
}

#endregion

app.Run();

// XUnit Inconsistent accessibility -> https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0
public partial class Program { }