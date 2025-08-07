using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Task;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class GetTasksByColumnQueryHandler : IRequestHandler<GetTasksByColumnQuery, List<TaskDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksByColumnQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TaskDto>> Handle(GetTasksByColumnQuery request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                return new List<TaskDto>();

            var tasks = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .WhereAsync(t => t.ColumnId == request.ColumnId && t.CompanyId == request.CompanyId);

            var taskIds = tasks.Select(t => t.Id).ToList();

            var labels = await _unitOfWork.Repository<TaskLabel>()
                .WhereAsync(l => taskIds.Contains(l.TaskId));

            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .WhereAsync(a => taskIds.Contains(a.TaskId));

            var attachments = await _unitOfWork.Repository<TaskAttachment>()
                .WhereAsync(a => taskIds.Contains(a.TaskId));

            var subtasks = await _unitOfWork.Repository<TaskSubtask>()
                .WhereAsync(s => taskIds.Contains(s.TaskId));

            var comments = await _unitOfWork.Repository<TaskComment>()
                .WhereAsync(c => taskIds.Contains(c.TaskId) && c.ParentCommentId == null);

            return tasks.OrderBy(t => t.CreatedAt).Select(t => new TaskDto
            {
                Id = t.Id.ToString(),
                Title = t.Title,
                Description = t.Description ?? "",
                ColumnId = t.ColumnId.ToString(),
                Author = new AuthorDto
                {
                    Id = t.AuthorId.ToString(),
                    Name = "Sofia Rivers", // TODO: Implementar servicio de usuarios
                    Username = "sofia.rivers",
                    Avatar = "/assets/avatar.png"
                },
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                Subscribed = t.IsSubscribed,
                Labels = labels.Where(l => l.TaskId == t.Id).Select(l => l.Name).ToList(),
                Assignees = assignees.Where(a => a.TaskId == t.Id).Select(a => new AssigneeDto
                {
                    Id = a.UserId.ToString(),
                    Name = a.Name,
                    Username = a.Username,
                    Avatar = a.Avatar
                }).ToList(),
                Attachments = attachments.Where(a => a.TaskId == t.Id).Select(a => new AttachmentDto
                {
                    Id = a.Id.ToString(),
                    Name = a.Name,
                    Extension = a.Extension,
                    Url = a.Url,
                    Size = a.Size
                }).ToList(),
                Subtasks = subtasks.Where(s => s.TaskId == t.Id)
                                  .OrderBy(s => s.DisplayOrder)
                                  .Select(s => new SubtaskDto
                                  {
                                      Id = s.Id.ToString(),
                                      Title = s.Title,
                                      Done = s.IsCompleted
                                  }).ToList(),
                Comments = comments.Where(c => c.TaskId == t.Id)
                                  .OrderBy(c => c.CreatedAt)
                                  .Select(c => new CommentDto
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
            }).ToList();
        }
    }
}
