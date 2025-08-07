using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            var comment = new TaskComment
            {
                Id = Guid.NewGuid(),
                TaskId = request.TaskId,
                AuthorId = Guid.Parse(request.UserId),
                AuthorName = "Current User", // TODO: Obtener de un servicio de usuarios
                AuthorUsername = "current.user",
                AuthorAvatar = "/assets/avatar.png",
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TaskComment>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return comment.Id;
        }
    }
}
