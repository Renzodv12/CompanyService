using MediatR;
using CompanyService.Core.DTOs;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener el detalle de un empleado específico
    /// </summary>
    public class GetEmployeeDetailQuery : IRequest<EmployeeDetailDto>
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}