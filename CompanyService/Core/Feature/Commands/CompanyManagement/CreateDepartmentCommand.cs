using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para crear un nuevo departamento
    /// </summary>
    public class CreateDepartmentCommand : IRequest<Guid>
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid UserId { get; set; }
    }
}