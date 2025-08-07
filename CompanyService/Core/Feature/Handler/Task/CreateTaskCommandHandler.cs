using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar que la columna existe
            var column = await _unitOfWork.Repository<TaskColumn>()
                .FirstOrDefaultAsync(c => c.Id == request.ColumnId && c.CompanyId == request.CompanyId);

            if (column == null)
                throw new DefaultException("La columna especificada no existe.");

            var task = new CompanyService.Core.Entities.Task
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                ColumnId = request.ColumnId,
                AuthorId = Guid.Parse(request.UserId),
                DueDate = request.DueDate,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Task>().AddAsync(task);

            // Agregar labels
            foreach (var labelName in request.Labels)
            {
                var label = new TaskLabel
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    Name = labelName
                };
                await _unitOfWork.Repository<TaskLabel>().AddAsync(label);
            }

            // TODO: Agregar assignees (necesitaríamos información de usuarios)

            await _unitOfWork.SaveChangesAsync();
            return task.Id;
        }
    }
}
