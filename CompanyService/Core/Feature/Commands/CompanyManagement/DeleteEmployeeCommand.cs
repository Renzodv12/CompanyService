using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    /// <summary>
    /// Comando para eliminar un empleado
    /// </summary>
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}