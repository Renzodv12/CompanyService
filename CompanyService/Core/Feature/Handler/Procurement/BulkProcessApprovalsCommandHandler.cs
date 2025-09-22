using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para procesar múltiples aprobaciones en lote
    /// </summary>
    public class BulkProcessApprovalsCommandHandler : IRequestHandler<BulkProcessApprovalsCommand, List<ApprovalResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BulkProcessApprovalsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ApprovalResponse>> Handle(BulkProcessApprovalsCommand request, CancellationToken cancellationToken)
        {
            var results = new List<ApprovalResponse>();

            // Buscar todas las aprobaciones
            var approvals = await _unitOfWork.Repository<Approval>()
                .WhereAsync(a => request.Request.ApprovalIds.Contains(a.Id) && a.CompanyId == request.CompanyId);

            if (!approvals.Any())
                throw new DefaultException("No se encontraron aprobaciones para procesar.");

            foreach (var approval in approvals)
            {
                // Procesar según la acción
                switch (request.Request.Action)
                {
                    case Core.Enums.ApprovalAction.Approve:
                        approval.Status = Core.Enums.ApprovalStatus.Approved;
                        approval.ApprovedDate = DateTime.UtcNow;
                        break;
                    case Core.Enums.ApprovalAction.Reject:
                        approval.Status = Core.Enums.ApprovalStatus.Rejected;
                        approval.RejectionReason = request.Request.Comments;
                        break;
                    case Core.Enums.ApprovalAction.Delegate:
                        if (request.Request.DelegateToUserId.HasValue)
                        {
                            approval.ApproverId = request.Request.DelegateToUserId.Value;
                            approval.Status = Core.Enums.ApprovalStatus.Pending;
                        }
                        break;
                }

                approval.Comments = request.Request.Comments;
                approval.LastModifiedAt = DateTime.UtcNow;

                _unitOfWork.Repository<Approval>().Update(approval);

                // Mapear a DTO
                results.Add(new ApprovalResponse
                {
                    Id = approval.Id,
                    CompanyId = approval.CompanyId,
                    CompanyName = approval.Company?.Name ?? "",
                    DocumentType = approval.DocumentType,
                    DocumentId = approval.DocumentId,
                    DocumentNumber = approval.DocumentNumber,
                    ApprovalLevelId = approval.ApprovalLevelId,
                    ApprovalLevelName = approval.ApprovalLevel?.Name ?? "",
                    Level = approval.ApprovalLevel?.Level ?? 0,
                    UserId = approval.ApproverId,
                    UserName = approval.ApproverUser?.Name ?? "",
                    Status = approval.Status,
                    DocumentAmount = approval.DocumentAmount,
                    Comments = approval.Comments,
                    RejectionReason = approval.RejectionReason,
                    DueDate = approval.DueDate,
                    ApprovedAt = approval.ApprovedDate,
                    CreatedAt = approval.CreatedAt,
                    UpdatedAt = approval.LastModifiedAt != default(DateTime) ? approval.LastModifiedAt : approval.CreatedAt
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return results;
        }
    }
}