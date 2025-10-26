using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Interfaces;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    public class CreateRestaurantTableCommand : IRequest<RestaurantTableDto>, IInvalidatesCache
    {
        public string TableNumber { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int Capacity { get; set; } = 4;
        public string? Location { get; set; }
        public string? Description { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }

        public string[] CachePatterns => new[]
        {
            $"restaurant-tables:company:{CompanyId}:restaurant:{RestaurantId}:*",
            $"available-tables:company:{CompanyId}:restaurant:{RestaurantId}:*",
            $"restaurant-dashboard:company:{CompanyId}:restaurant:{RestaurantId}"
        };
    }
}
