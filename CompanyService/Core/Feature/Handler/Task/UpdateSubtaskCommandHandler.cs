using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class UpdateSubtaskCommandHandler : IRequestHandler<UpdateSubtaskCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubtaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateSubtaskCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            var subtask = await _unitOfWork.Repository<TaskSubtask>()
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.TaskId == request.TaskId);

            if (subtask == null)
                throw new DefaultException("Subtarea no encontrada.");

            subtask.Title = request.Title;
            subtask.IsCompleted = request.Done;

            _unitOfWork.Repository<TaskSubtask>().Update(subtask);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
