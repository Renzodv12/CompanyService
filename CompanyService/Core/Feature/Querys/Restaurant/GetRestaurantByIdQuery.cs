using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetRestaurantByIdQuery : IRequest<RestaurantDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
    }
}
