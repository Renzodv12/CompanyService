using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Task
{
    public class GetTaskByIdQuery : IRequest<TaskDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
