using MediatR;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para procesar una aprobación (aprobar/rechazar)
    /// </summary>
    public class ProcessApprovalCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public ApprovalAction Action { get; set; }
        public string Comments { get; set; } = string.Empty;
        public Guid? DelegateToUserId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}