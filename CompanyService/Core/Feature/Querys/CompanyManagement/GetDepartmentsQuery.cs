using MediatR;
using CompanyService.Core.DTOs.Department;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener la lista de departamentos
    /// </summary>
    public class GetDepartmentsQuery : IRequest<List<DepartmentDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}