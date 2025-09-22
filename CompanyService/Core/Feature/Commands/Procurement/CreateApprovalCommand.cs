using MediatR;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para crear una nueva aprobación
    /// </summary>
    public class CreateApprovalCommand : IRequest<Guid>
    {
        public string DocumentType { get; set; } = string.Empty;
        public Guid DocumentId { get; set; }
        public string? DocumentNumber { get; set; }
        public Guid ApprovalLevelId { get; set; }
        public Guid UserId { get; set; }
        public decimal? DocumentAmount { get; set; }
        public string? Comments { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid CompanyId { get; set; }
        public Guid RequestingUserId { get; set; }
    }
}