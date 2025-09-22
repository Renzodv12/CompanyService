using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para crear una nueva posición
    /// </summary>
    public class CreatePositionCommand : IRequest<Guid>
    {
        public Guid CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? DepartmentId { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Requirements { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid UserId { get; set; }
    }
}