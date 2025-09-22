using MediatR;
using CompanyService.Core.DTOs.CRM;

namespace CompanyService.Core.Feature.Querys.CRM
{
    /// <summary>
    /// Query para obtener un lead específico
    /// </summary>
    public class GetLeadQuery : IRequest<LeadDto>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}