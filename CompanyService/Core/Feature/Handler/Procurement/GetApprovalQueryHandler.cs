using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener una aprobación por ID
    /// </summary>
    public class GetApprovalQueryHandler : IRequestHandler<GetApprovalQuery, ApprovalResponse?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetApprovalQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApprovalResponse?> Handle(GetApprovalQuery request, CancellationToken cancellationToken)
        {
            var approval = await _unitOfWork.Repository<Approval>()
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == request.CompanyId);

            if (approval == null)
                return null;

            return new ApprovalResponse
            {
                Id = approval.Id,
                CompanyId = approval.CompanyId,
                DocumentType = approval.DocumentType,
                DocumentId = approval.DocumentId,
                UserId = approval.RequestedBy,
                Status = approval.Status,
                CreatedAt = approval.RequestDate,
                ApprovedAt = approval.ApprovedDate,
                Comments = approval.Comments,
                DueDate = approval.DueDate
            };
        }
    }
}