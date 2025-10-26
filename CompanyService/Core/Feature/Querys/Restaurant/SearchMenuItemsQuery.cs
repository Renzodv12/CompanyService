using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class SearchMenuItemsQuery : IRequest<List<RestaurantMenuItemDto>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
