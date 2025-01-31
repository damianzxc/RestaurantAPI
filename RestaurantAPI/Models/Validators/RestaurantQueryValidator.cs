using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] _allowedPageSizes = [5, 10, 15];
        private string[] _allowedSortByColumnNames = 
            [nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category)];

        public RestaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!_allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"Page size must be in [{string.Join(",", _allowedPageSizes)}]");
                }
            });
            RuleFor(r => r.SortBy).Custom((value, context) =>
            {
                if (string.IsNullOrEmpty(value) || !_allowedSortByColumnNames.Contains(value))
                {
                    context.AddFailure("SortBy", $"Sort key is optional or must by one of [{string.Join(",", _allowedSortByColumnNames)}]");
                }
            });
        }
    }
}
