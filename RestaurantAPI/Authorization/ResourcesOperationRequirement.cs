using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    // How operation do you need?
    public enum ResourceOperation
    { 
        Create,
        Read,
        Update,
        Delete,
    }
    public class ResourcesOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperation ResourceOperation { get; }
        public ResourcesOperationRequirement(ResourceOperation resourceOperation)
        {
             ResourceOperation = resourceOperation;
        }
    }
}
