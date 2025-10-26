using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetRestaurantDashboardQuery : IRequest<RestaurantDashboardDto>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }
}
