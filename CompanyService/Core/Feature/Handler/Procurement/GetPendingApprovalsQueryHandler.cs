using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener las aprobaciones pendientes
    /// </summary>
    public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, PagedResult<ApprovalResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPendingApprovalsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ApprovalResponse>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            // Obtener todas las aprobaciones que coincidan con el filtro
            var allApprovals = await _unitOfWork.Repository<Approval>()
                .WhereAsync(a => a.CompanyId == request.CompanyId && a.Status == ApprovalStatus.Pending);

            // Obtener total de registros
            var totalCount = allApprovals.Count();

            // Aplicar paginación y ordenamiento
            var approvals = allApprovals
                .OrderBy(a => a.RequestedDate) // Las más antiguas primero
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Mapear a ApprovalResponse
            var approvalResponses = approvals.Select(a => new ApprovalResponse
            {
                Id = a.Id,
                CompanyId = a.CompanyId,
                DocumentType = a.DocumentType,
                DocumentId = a.DocumentId,
                UserId = a.RequestedBy,
                Status = a.Status,
                CreatedAt = a.RequestedDate,
                ApprovedAt = a.ApprovedDate,
                Comments = a.Comments,
                DueDate = a.DueDate
            }).ToList();

            return new PagedResult<ApprovalResponse>
            {
                Items = approvalResponses,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}