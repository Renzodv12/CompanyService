using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetAvailableTablesQuery : IRequest<List<RestaurantTableDto>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public int? MinCapacity { get; set; }
    }
}
