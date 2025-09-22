using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para crear una nueva aprobación
    /// </summary>
    public class CreateApprovalCommandHandler : IRequestHandler<CreateApprovalCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateApprovalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateApprovalCommand request, CancellationToken cancellationToken)
        {
            // Validar que la empresa existe
            var company = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>()
                .FirstOrDefaultAsync(c => c.Id == request.CompanyId);

            if (company == null)
                throw new DefaultException("Empresa no encontrada.");

            // Crear la aprobación
            var approval = new Approval
            {
                Id = Guid.NewGuid(),
                DocumentType = request.DocumentType,
                DocumentId = request.DocumentId,
                RequestedBy = request.UserId,
                Status = ApprovalStatus.Pending,
                RequestedDate = DateTime.UtcNow,
                CompanyId = request.CompanyId,
                CreatedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Approval>().AddAsync(approval);
            await _unitOfWork.SaveChangesAsync();

            return approval.Id;
        }
    }
}