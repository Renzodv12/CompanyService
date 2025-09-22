using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para crear un nuevo nivel de aprobación
    /// </summary>
    public class CreateApprovalLevelCommandHandler : IRequestHandler<CreateApprovalLevelCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateApprovalLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateApprovalLevelCommand request, CancellationToken cancellationToken)
        {
            // Validar que la empresa existe
            var company = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>()
                .FirstOrDefaultAsync(c => c.Id == request.CompanyId);

            if (company == null)
                throw new DefaultException("Empresa no encontrada.");

            // Validar que no existe un nivel con el mismo número para la empresa
            var existingLevel = await _unitOfWork.Repository<ApprovalLevel>()
                .FirstOrDefaultAsync(al => al.Level == request.Level && al.CompanyId == request.CompanyId);

            if (existingLevel != null)
                throw new DefaultException($"Ya existe un nivel de aprobación {request.Level} para esta empresa.");

            // Crear el nivel de aprobación
            var approvalLevel = new ApprovalLevel
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Level = request.Level,
                MinAmount = request.MinAmount,
                MaxAmount = request.MaxAmount,
                RequiresAllApprovers = request.RequiresAllApprovers,
                IsActive = request.IsActive,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ApprovalLevel>().AddAsync(approvalLevel);
            await _unitOfWork.SaveChangesAsync();

            return approvalLevel.Id;
        }
    }
}