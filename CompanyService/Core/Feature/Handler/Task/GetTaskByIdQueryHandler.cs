using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Task;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                return null;

            var task = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId);

            if (task == null)
                return null;

            var labels = await _unitOfWork.Repository<TaskLabel>()
                .WhereAsync(l => l.TaskId == task.Id);

            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .WhereAsync(a => a.TaskId == task.Id);

            var attachments = await _unitOfWork.Repository<TaskAttachment>()
                .WhereAsync(a => a.TaskId == task.Id);

            var subtasks = await _unitOfWork.Repository<TaskSubtask>()
                .WhereAsync(s => s.TaskId == task.Id);

            var comments = await _unitOfWork.Repository<TaskComment>()
                .WhereAsync(c => c.TaskId == task.Id && c.ParentCommentId == null);

            return new TaskDto
            {
                Id = task.Id.ToString(),
                Title = task.Title,
                Description = task.Description ?? "",
                ColumnId = task.ColumnId.ToString(),
                Author = new AuthorDto
                {
                    Id = task.AuthorId.ToString(),
                    Name = "Sofia Rivers",
                    Username = "sofia.rivers",
                    Avatar = "/assets/avatar.png"
                },
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                Subscribed = task.IsSubscribed,
                Labels = labels.Select(l => l.Name).ToList(),
                Assignees = assignees.Select(a => new AssigneeDto
                {
                    Id = a.UserId.ToString(),
                    Name = a.Name,
                    Username = a.Username,
                    Avatar = a.Avatar
                }).ToList(),
                Attachments = attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id.ToString(),
                    Name = a.Name,
                    Extension = a.Extension,
                    Url = a.Url,
                    Size = a.Size
                }).ToList(),
                Subtasks = subtasks.OrderBy(s => s.DisplayOrder).Select(s => new SubtaskDto
                {
                    Id = s.Id.ToString(),
                    Title = s.Title,
                    Done = s.IsCompleted
                }).ToList(),
                Comments = comments.OrderBy(c => c.CreatedAt).Select(c => new CommentDto
                {
                    Id = c.Id.ToString(),
                    Author = new AuthorDto
                    {
                        Id = c.AuthorId.ToString(),
                        Name = c.AuthorName,
                        Username = c.AuthorUsername,
                        Avatar = c.AuthorAvatar
                    },
                    CreatedAt = c.CreatedAt,
                    Content = c.Content,
                    Comments = new List<CommentDto>()
                }).ToList()
            };
        }
    }
}
