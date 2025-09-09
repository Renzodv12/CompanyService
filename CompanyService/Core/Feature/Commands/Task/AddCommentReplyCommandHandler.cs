using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class AddCommentReplyCommandHandler : IRequestHandler<AddCommentReplyCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCommentReplyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddCommentReplyCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar que el comentario padre existe y pertenece a la tarea
            var parentComment = await _unitOfWork.Repository<TaskComment>()
                .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId && c.TaskId == request.TaskId);

            if (parentComment == null)
                throw new DefaultException("Comentario padre no encontrado.");

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == request.CompanyId);

            if (task == null)
                throw new DefaultException("Tarea no encontrada.");

            var reply = new TaskComment
            {
                Id = Guid.NewGuid(),
                TaskId = request.TaskId,
                ParentCommentId = request.ParentCommentId,
                AuthorId = Guid.Parse(request.UserId),
                AuthorName = "Usuario Desconocido",
                AuthorUsername = "unknown.user",
                AuthorAvatar = "/assets/avatar.png",
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<TaskComment>().AddAsync(reply);
            await _unitOfWork.SaveChangesAsync();

            return reply.Id;
        }
    }
}
