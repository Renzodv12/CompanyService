using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class CreateSubtaskCommandHandler : IRequestHandler<CreateSubtaskCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubtaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            // Obtener el siguiente orden de display
            var existingSubtasks = await _unitOfWork.Repository<TaskSubtask>()
                .WhereAsync(s => s.TaskId == request.TaskId);

            var maxOrder = existingSubtasks.Any() ? existingSubtasks.Max(s => s.DisplayOrder) : 0;

            var subtask = new TaskSubtask
            {
                Id = Guid.NewGuid(),
                TaskId = request.TaskId,
                Title = request.Title,
                IsCompleted = false,
                DisplayOrder = maxOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TaskSubtask>().AddAsync(subtask);
            await _unitOfWork.SaveChangesAsync();

            return subtask.Id;
        }
    }
}
