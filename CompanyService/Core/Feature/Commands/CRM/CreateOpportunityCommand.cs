using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para crear una nueva oportunidad
    /// </summary>
    public class CreateOpportunityCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? EstimatedValue { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public string Stage { get; set; } = string.Empty;
        public decimal? Probability { get; set; }
        public Guid? LeadId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}