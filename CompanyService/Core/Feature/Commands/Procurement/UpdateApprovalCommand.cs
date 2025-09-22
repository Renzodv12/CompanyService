using MediatR;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para actualizar una aprobación existente
    /// </summary>
    public class UpdateApprovalCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? Comments { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}