using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para remover un lead de una campaña
    /// </summary>
    public class RemoveLeadFromCampaignCommand : IRequest<bool>
    {
        public Guid CampaignId { get; set; }
        public Guid LeadId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}