using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para actualizar una sucursal existente
    /// </summary>
    public class UpdateBranchCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? ManagerId { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
    }
}