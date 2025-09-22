using MediatR;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para eliminar una aprobación
    /// </summary>
    public class DeleteApprovalCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}