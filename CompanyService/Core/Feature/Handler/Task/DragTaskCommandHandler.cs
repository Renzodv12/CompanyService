using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class DragTaskCommandHandler : IRequestHandler<DragTaskCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DragTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DragTaskCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            // Cambiar columna de la tarea
            task.ColumnId = request.TargetColumnId;
            task.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CompanyService.Core.Entities.Task>().Update(task);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
