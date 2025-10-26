using MediatR;
using CompanyService.Core.Interfaces;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    public class DeleteRestaurantCommand : IRequest<bool>, IInvalidatesCache
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }

        public string[] CachePatterns => new[]
        {
            $"restaurants:company:{CompanyId}:*",
            $"restaurant:company:{CompanyId}:id:{Id}",
            $"restaurant-dashboard:company:{CompanyId}:*",
            $"restaurant-tables:company:{CompanyId}:*",
            $"available-tables:company:{CompanyId}:*"
        };
    }
}
