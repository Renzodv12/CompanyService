using MediatR;
using CompanyService.Core.DTOs.Branch;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener la lista de sucursales
    /// </summary>
    public class GetBranchesQuery : IRequest<List<BranchDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}