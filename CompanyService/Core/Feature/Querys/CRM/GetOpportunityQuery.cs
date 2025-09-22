using MediatR;
using CompanyService.Core.DTOs.CRM;

namespace CompanyService.Core.Feature.Querys.CRM
{
    /// <summary>
    /// Query para obtener una oportunidad específica
    /// </summary>
    public class GetOpportunityQuery : IRequest<OpportunityDto>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}