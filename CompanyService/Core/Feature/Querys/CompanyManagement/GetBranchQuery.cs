using MediatR;
using CompanyService.Core.DTOs.Branch;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener una sucursal específica
    /// </summary>
    public class GetBranchQuery : IRequest<BranchDetailDto>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}