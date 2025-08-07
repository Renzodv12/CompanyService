using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateColumnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener el siguiente orden de display
            var existingColumns = await _unitOfWork.Repository<TaskColumn>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            var maxOrder = existingColumns.Any() ? existingColumns.Max(c => c.DisplayOrder) : 0;

            var column = new TaskColumn
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DisplayOrder = maxOrder + 1,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TaskColumn>().AddAsync(column);
            await _unitOfWork.SaveChangesAsync();

            return column.Id;
        }
    }
}
