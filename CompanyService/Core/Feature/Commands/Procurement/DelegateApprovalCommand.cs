using MediatR;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para delegar aprobaciones
    /// </summary>
    public class DelegateApprovalCommand : IRequest<bool>
    {
        public Guid CompanyId { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Guid>? ApprovalLevelIds { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}