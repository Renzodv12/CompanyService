using MediatR;
using CompanyService.Core.DTOs.CRM;

namespace CompanyService.Core.Feature.Querys.CRM
{
    /// <summary>
    /// Query para obtener la lista de campañas
    /// </summary>
    public class GetCampaignsQuery : IRequest<List<CampaignDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}