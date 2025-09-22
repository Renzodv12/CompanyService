using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para actualizar un nivel de aprobación existente
    /// </summary>
    public class UpdateApprovalLevelCommandHandler : IRequestHandler<UpdateApprovalLevelCommand, ApprovalLevelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateApprovalLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApprovalLevelResponse> Handle(UpdateApprovalLevelCommand request, CancellationToken cancellationToken)
        {
            // Buscar el nivel de aprobación existente
            var approvalLevel = await _unitOfWork.Repository<ApprovalLevel>()
                .GetByIdAsync(request.Id);

            if (approvalLevel == null || approvalLevel.CompanyId != request.CompanyId)
                throw new DefaultException("Nivel de aprobación no encontrado.");

            // Actualizar campos
            approvalLevel.Name = request.Request.Name;
            approvalLevel.Description = request.Request.Description;
            approvalLevel.Level = request.Request.Level;
            approvalLevel.MinAmount = request.Request.MinAmount;
            approvalLevel.MaxAmount = request.Request.MaxAmount;
            approvalLevel.RequiresAllApprovers = request.Request.RequiresAllApprovers;
            approvalLevel.RequiredApprovals = request.Request.RequiredApprovals;
            // AutoApprovalDays no existe en la entidad
            approvalLevel.IsActive = request.Request.IsActive;
            approvalLevel.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ApprovalLevel>().Update(approvalLevel);
            await _unitOfWork.SaveChangesAsync();

            // Mapear a DTO
            return new ApprovalLevelResponse
            {
                Id = approvalLevel.Id,
                CompanyId = approvalLevel.CompanyId,
                CompanyName = approvalLevel.Company?.Name ?? "",
                Name = approvalLevel.Name,
                Description = approvalLevel.Description,
                DocumentType = approvalLevel.DocumentType,
                Level = approvalLevel.Level,
                MinAmount = approvalLevel.MinAmount,
                MaxAmount = approvalLevel.MaxAmount,
                RequiresAllApprovers = approvalLevel.RequiresAllApprovers,
                RequiredApprovals = approvalLevel.RequiredApprovals,
                // AutoApprovalDays no existe en la entidad
                IsActive = approvalLevel.IsActive,
                CreatedAt = approvalLevel.CreatedAt,
                UpdatedAt = approvalLevel.LastModifiedAt
            };
        }
    }
}