using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Interfaces;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    public class ChangeTableStatusCommand : IRequest<RestaurantTableDto>, IInvalidatesCache
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }

        public string[] CachePatterns => new[]
        {
            $"restaurant-tables:company:{CompanyId}:restaurant:{RestaurantId}:*",
            $"restaurant-table:company:{CompanyId}:restaurant:{RestaurantId}:table:{Id}",
            $"available-tables:company:{CompanyId}:restaurant:{RestaurantId}:*",
            $"restaurant-dashboard:company:{CompanyId}:restaurant:{RestaurantId}"
        };
    }
}
