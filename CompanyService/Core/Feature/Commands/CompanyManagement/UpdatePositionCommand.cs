using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para actualizar una posición existente
    /// </summary>
    public class UpdatePositionCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? DepartmentId { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Requirements { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
    }
}