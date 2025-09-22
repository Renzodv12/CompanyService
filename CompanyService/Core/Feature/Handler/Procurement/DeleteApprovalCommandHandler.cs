using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para eliminar una aprobación
    /// </summary>
    public class DeleteApprovalCommandHandler : IRequestHandler<DeleteApprovalCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteApprovalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteApprovalCommand request, CancellationToken cancellationToken)
        {
            // Buscar la aprobación
            var approval = await _unitOfWork.Repository<Approval>()
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == request.CompanyId);

            if (approval == null)
                throw new DefaultException("Aprobación no encontrada.");

            // Verificar que solo se puedan eliminar aprobaciones pendientes
            if (approval.Status != ApprovalStatus.Pending)
                throw new DefaultException("Solo se pueden eliminar aprobaciones pendientes.");

            _unitOfWork.Repository<Approval>().Remove(approval);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}