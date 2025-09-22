using MediatR;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.CRM;

namespace CompanyService.Core.Feature.Querys.CRM
{
    /// <summary>
    /// Query para obtener la lista de oportunidades
    /// </summary>
    public class GetOpportunitiesQuery : IRequest<PagedResult<OpportunityDto>>
    {
        public Guid CompanyId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid UserId { get; set; }
    }
}