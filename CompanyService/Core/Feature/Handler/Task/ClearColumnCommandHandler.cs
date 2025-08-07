using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class ClearColumnCommandHandler : IRequestHandler<ClearColumnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClearColumnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ClearColumnCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var column = await _unitOfWork.Repository<TaskColumn>()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId);

            if (column == null)
                throw new DefaultException("Columna no encontrada.");

            // Eliminar solo las tareas de la columna, no la columna misma
            var tasks = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .WhereAsync(t => t.ColumnId == request.Id);

            foreach (var task in tasks)
            {
                // Eliminar entidades relacionadas
                var labels = await _unitOfWork.Repository<TaskLabel>().WhereAsync(l => l.TaskId == task.Id);
                var assignees = await _unitOfWork.Repository<TaskAssignee>().WhereAsync(a => a.TaskId == task.Id);
                var attachments = await _unitOfWork.Repository<TaskAttachment>().WhereAsync(a => a.TaskId == task.Id);
                var subtasks = await _unitOfWork.Repository<TaskSubtask>().WhereAsync(s => s.TaskId == task.Id);
                var comments = await _unitOfWork.Repository<TaskComment>().WhereAsync(c => c.TaskId == task.Id);

                foreach (var label in labels) _unitOfWork.Repository<TaskLabel>().Remove(label);
                foreach (var assignee in assignees) _unitOfWork.Repository<TaskAssignee>().Remove(assignee);
                foreach (var attachment in attachments) _unitOfWork.Repository<TaskAttachment>().Remove(attachment);
                foreach (var subtask in subtasks) _unitOfWork.Repository<TaskSubtask>().Remove(subtask);
                foreach (var comment in comments) _unitOfWork.Repository<TaskComment>().Remove(comment);

                _unitOfWork.Repository<CompanyService.Core.Entities.Task>().Remove(task);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
