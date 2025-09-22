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
    /// Handler para obtener la lista de aprobaciones con filtros
    /// </summary>
    public class GetApprovalsQueryHandler : IRequestHandler<GetApprovalsQuery, PagedResult<ApprovalResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetApprovalsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ApprovalResponse>> Handle(GetApprovalsQuery request, CancellationToken cancellationToken)
        {
            // Obtener todas las aprobaciones de la compañía
            var allApprovals = await _unitOfWork.Repository<Approval>()
                .WhereAsync(a => a.CompanyId == request.CompanyId);

            // Aplicar filtros
            if (request.Status.HasValue)
            {
                allApprovals = allApprovals.Where(a => a.Status == request.Status.Value);
            }

            if (!string.IsNullOrEmpty(request.DocumentType))
            {
                allApprovals = allApprovals.Where(a => a.DocumentType == request.DocumentType);
            }

            // Obtener total de registros
            var totalCount = allApprovals.Count();

            // Aplicar paginación
            var approvals = allApprovals
                .OrderByDescending(a => a.RequestedDate)
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