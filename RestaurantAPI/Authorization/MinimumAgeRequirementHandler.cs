using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequired>
    {
        private readonly ILogger<MinimumAgeRequirementHandler> _logger;
        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger) 
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequired requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth")?.Value);

            // ILogger info
            var userEmail = context.User.FindFirst(c => c.Type == "DateOfBirth")?.Value;
            _logger.LogInformation($"User: {userEmail} with date of birth [{dateOfBirth}]");

            if (dateOfBirth.AddYears(requirement.MinimumAge) > DateTime.Today)
            { 
                context.Succeed(requirement);
                _logger.LogInformation("Authorization succedded");
            }
            else
            {
                _logger.LogInformation("Authorization failed");
            }

            return Task.CompletedTask;
        }
    }
}
