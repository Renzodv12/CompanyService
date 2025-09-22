using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para eliminar una campaña
    /// </summary>
    public class DeleteCampaignCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}