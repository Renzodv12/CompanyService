using MediatR;
using CompanyService.Core.DTOs;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener un empleado específico
    /// </summary>
    public class GetEmployeeQuery : IRequest<EmployeeDetailDto>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}