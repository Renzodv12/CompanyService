using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para agregar un lead a una campaña
    /// </summary>
    public class AddLeadToCampaignCommand : IRequest<bool>
    {
        public Guid CampaignId { get; set; }
        public Guid LeadId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}