using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetRestaurantTableByIdQuery : IRequest<RestaurantTableDto?>
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }
}
