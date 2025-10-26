using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Interfaces;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    public class UpdateRestaurantCommand : IRequest<RestaurantDto>, IInvalidatesCache
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? RUC { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }

        public string[] CachePatterns => new[]
        {
            $"restaurants:company:{CompanyId}:*",
            $"restaurant:company:{CompanyId}:id:{Id}",
            $"restaurant-dashboard:company:{CompanyId}:*"
        };
    }
}
