using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetRestaurantMenuItemsQuery : IRequest<List<RestaurantMenuItemDto>>
    {
        public Guid MenuId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public string? Category { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsVegetarian { get; set; }
        public bool? IsVegan { get; set; }
        public bool? IsGlutenFree { get; set; }
    }
}
