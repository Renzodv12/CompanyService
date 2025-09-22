using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para eliminar un departamento
    /// </summary>
    public class DeleteDepartmentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}