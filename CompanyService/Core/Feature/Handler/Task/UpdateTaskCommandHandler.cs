using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            // Actualizar task
            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CompanyService.Core.Entities.Task>().Update(task);

            // Actualizar labels (eliminar existentes y agregar nuevos)
            var existingLabels = await _unitOfWork.Repository<TaskLabel>()
                .WhereAsync(l => l.TaskId == request.Id);

            foreach (var label in existingLabels)
            {
                _unitOfWork.Repository<TaskLabel>().Remove(label);
            }

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

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
