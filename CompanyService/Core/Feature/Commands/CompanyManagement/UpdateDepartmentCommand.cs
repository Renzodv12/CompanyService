using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para actualizar un departamento existente
    /// </summary>
    public class UpdateDepartmentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? BranchId { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
    }
}