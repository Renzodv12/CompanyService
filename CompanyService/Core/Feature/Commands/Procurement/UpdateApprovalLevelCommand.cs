using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para actualizar un nivel de aprobación existente
    /// </summary>
    public class UpdateApprovalLevelCommand : IRequest<ApprovalLevelResponse>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public UpdateApprovalLevelRequest Request { get; set; } = new();
    }
}