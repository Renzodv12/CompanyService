using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para convertir un lead en una oportunidad
    /// </summary>
    public class ConvertLeadToOpportunityCommand : IRequest<Guid>
    {
        public Guid LeadId { get; set; }
        public string OpportunityName { get; set; } = string.Empty;
        public decimal? EstimatedValue { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}