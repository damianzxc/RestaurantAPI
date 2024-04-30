using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
using RestaurantAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Authentication Settings obj
var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

// We can inject this obj to use as service
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;   // Nie wymuszamy https
    cfg.SaveToken = true;   // Info że token powinien zostać zapisany po stronie serwera
    cfg.TokenValidationParameters = new TokenValidationParameters   // Validacja tokena
    {
        ValidIssuer = authenticationSettings.JwtIssuer,     // Wydawca danego tokenu
        ValidAudience = authenticationSettings.JwtIssuer,   // Jakie podmioty mogą go używać (w obrębie 1 aplikacja wzkazujemy na siebie)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),     // Klucz prywatny na podstawie appsettings.json
    };
});

// Own Authorization
builder.Services.AddAuthorization(options =>
{                                                                       // Claim nationality with value France or Polish
    options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "France", "Polish"));

    // How to add own policy (this policy prevent users with age below 20 to access)
    options.AddPolicy("AddLeast20", builder => builder.AddRequirements(new MinimumAgeRequired(20)));
});
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();


// Add services to the container.
builder.Services.AddControllers();
// 2. Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDto>();    // Register Validators
builder.Services.AddFluentValidationAutoValidation();                       // The same old MVC pipeline behavior
builder.Services.AddFluentValidationClientsideAdapters();                   // For client side

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
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
// Add FluentValidation
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();

// Logger Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMeasure>();

// Swagger
builder.Services.AddSwaggerGen();

// Add NLog
builder.Host.UseNLog();

var app = builder.Build();

// Logger Middleware Registration (before UseHttpRedirection!)
//app.UseErrorHandlingMiddleware(); (=> see Example with static class)
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRequestTimeMeasure();

// All request have to use authentication. Above UseHttpRecirection!
app.UseAuthentication();

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