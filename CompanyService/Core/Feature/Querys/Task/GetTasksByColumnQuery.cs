using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Task
{
    public class GetTasksByColumnQuery : IRequest<List<TaskDto>>
    {
        public Guid ColumnId { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
