using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgeRequired : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequired(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}
