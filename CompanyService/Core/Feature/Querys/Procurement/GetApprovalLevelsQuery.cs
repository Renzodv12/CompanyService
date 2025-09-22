using MediatR;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    /// <summary>
    /// Query para obtener los niveles de aprobación
    /// </summary>
    public class GetApprovalLevelsQuery : IRequest<PagedResult<ApprovalLevelResponse>>
    {
        public Guid CompanyId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid UserId { get; set; }
    }
}