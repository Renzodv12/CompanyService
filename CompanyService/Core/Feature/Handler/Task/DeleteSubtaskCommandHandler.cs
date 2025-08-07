using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class DeleteSubtaskCommandHandler : IRequestHandler<DeleteSubtaskCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubtaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSubtaskCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var subtask = await _unitOfWork.Repository<TaskSubtask>()
                .FirstOrDefaultAsync(s => s.Id == request.Id);

            if (subtask == null)
                throw new DefaultException("Subtarea no encontrada.");

            // Verificar que la subtarea pertenece a una tarea de la empresa
            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == subtask.TaskId && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Acceso denegado.");

            _unitOfWork.Repository<TaskSubtask>().Remove(subtask);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
