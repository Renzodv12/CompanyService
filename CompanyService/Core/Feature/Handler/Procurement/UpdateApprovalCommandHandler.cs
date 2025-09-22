using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para actualizar una aprobación existente
    /// </summary>
    public class UpdateApprovalCommandHandler : IRequestHandler<UpdateApprovalCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateApprovalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateApprovalCommand request, CancellationToken cancellationToken)
        {
            // Buscar la aprobación
            var approval = await _unitOfWork.Repository<Approval>()
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == request.CompanyId);

            if (approval == null)
                throw new DefaultException("Aprobación no encontrada.");

            // Actualizar campos
            if (!string.IsNullOrEmpty(request.Comments))
                approval.Comments = request.Comments;

            if (request.DueDate.HasValue)
                approval.DueDate = request.DueDate.Value;

            approval.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Approval>().Update(approval);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}