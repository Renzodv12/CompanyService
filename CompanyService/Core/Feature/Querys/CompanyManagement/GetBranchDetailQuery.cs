using MediatR;
using CompanyService.Core.DTOs.Branch;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener el detalle de una sucursal específica
    /// </summary>
    public class GetBranchDetailQuery : IRequest<BranchDetailDto>
    {
        public Guid BranchId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}