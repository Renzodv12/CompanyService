using MediatR;
using CompanyService.Core.DTOs;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener la lista de empleados
    /// </summary>
    public class GetEmployeesQuery : IRequest<List<EmployeeDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}