using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            // Eliminar entidades relacionadas
            var labels = await _unitOfWork.Repository<TaskLabel>()
                .WhereAsync(l => l.TaskId == request.Id);
            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .WhereAsync(a => a.TaskId == request.Id);
            var attachments = await _unitOfWork.Repository<TaskAttachment>()
                .WhereAsync(a => a.TaskId == request.Id);
            var subtasks = await _unitOfWork.Repository<TaskSubtask>()
                .WhereAsync(s => s.TaskId == request.Id);
            var comments = await _unitOfWork.Repository<TaskComment>()
                .WhereAsync(c => c.TaskId == request.Id);

            // Eliminar todo
            foreach (var label in labels) _unitOfWork.Repository<TaskLabel>().Remove(label);
            foreach (var assignee in assignees) _unitOfWork.Repository<TaskAssignee>().Remove(assignee);
            foreach (var attachment in attachments) _unitOfWork.Repository<TaskAttachment>().Remove(attachment);
            foreach (var subtask in subtasks) _unitOfWork.Repository<TaskSubtask>().Remove(subtask);
            foreach (var comment in comments) _unitOfWork.Repository<TaskComment>().Remove(comment);

            _unitOfWork.Repository<CompanyService.Core.Entities.Task>().Remove(task);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
