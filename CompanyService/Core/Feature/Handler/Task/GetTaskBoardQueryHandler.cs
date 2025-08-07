using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Task;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Task;
using CompanyService.Core.Utils;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class GetTaskBoardQueryHandler : IRequestHandler<GetTaskBoardQuery, TaskBoardResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTaskBoardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskBoardResponse> Handle(GetTaskBoardQuery request, CancellationToken cancellationToken)
        {
            // Validar acceso
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                return new TaskBoardResponse();

            // Obtener columnas
            var columns = await _unitOfWork.Repository<TaskColumn>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            // Obtener tareas
            var tasks = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .WhereAsync(t => t.CompanyId == request.CompanyId);

            // Obtener labels, assignees, attachments, subtasks, comments
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
            // Obtener TODOS los comentarios (incluyendo replies)
            var allComments = await _unitOfWork.Repository<TaskComment>()
                .WhereAsync(c => taskIds.Contains(c.TaskId));
            var columnDtos = columns
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new ColumnDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    TaskIds = tasks.Where(t => t.ColumnId == c.Id)
                                  .OrderBy(t => t.CreatedAt)
                                  .Select(t => t.Id.ToString())
                                  .ToList()
                }).ToList();
          
            var taskDtos = tasks.Select(t => 
            {
                   //var authorInfo = authorLookup.GetValueOrDefault(t.AuthorId);
        var taskComments = allComments.Where(c => c.TaskId == t.Id);
                return new TaskDto
                {
                    Id = t.Id.ToString(),
                    Title = t.Title,
                    Description = t.Description ?? "",
                    ColumnId = t.ColumnId.ToString(),
                    Author = new AuthorDto
                    {
                        Id = t.AuthorId.ToString(),
                        Name = "Sofia Rivers", // TODO: Obtener datos reales del usuario
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
                    Comments = CommentHelper.BuildNestedComments(taskComments) // Usar el helper

                };
            }).ToList();

            return new TaskBoardResponse
            {
                Columns = columnDtos,
                Tasks = taskDtos
            };
        }
    }
}
