using MediatR;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para eliminar un nivel de aprobación
    /// </summary>
    public class DeleteApprovalLevelCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}