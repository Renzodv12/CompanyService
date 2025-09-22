using MediatR;
using CompanyService.Core.DTOs.CRM;

namespace CompanyService.Core.Feature.Querys.CRM
{
    /// <summary>
    /// Query para obtener el dashboard de CRM
    /// </summary>
    public class GetCRMDashboardQuery : IRequest<CRMDashboardDto>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}