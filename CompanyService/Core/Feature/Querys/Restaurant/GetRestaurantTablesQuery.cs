using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetRestaurantTablesQuery : IRequest<List<RestaurantTableDto>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
