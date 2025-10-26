using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Interfaces;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    public class CreateRestaurantCommand : IRequest<RestaurantDto>, IInvalidatesCache
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? RUC { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }

        public string[] CachePatterns => new[]
        {
            $"restaurants:company:{CompanyId}:*",
            $"restaurant-dashboard:company:{CompanyId}:*"
        };
    }
}
