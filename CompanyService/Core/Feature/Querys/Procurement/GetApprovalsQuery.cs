using MediatR;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    /// <summary>
    /// Query para obtener la lista de aprobaciones
    /// </summary>
    public class GetApprovalsQuery : IRequest<PagedResult<ApprovalResponse>>
    {
        public Guid CompanyId { get; set; }
        public ApprovalStatus? Status { get; set; }
        public string? DocumentType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid UserId { get; set; }
        public ApprovalFilterRequest? Filter { get; set; }
    }
}