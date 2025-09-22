using MediatR;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Comando para crear un nuevo nivel de aprobación
    /// </summary>
    public class CreateApprovalLevelCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Level { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool RequiresAllApprovers { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}