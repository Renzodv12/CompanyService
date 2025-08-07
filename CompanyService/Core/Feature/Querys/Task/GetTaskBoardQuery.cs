using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Task
{
    public class GetTaskBoardQuery : IRequest<TaskBoardResponse>
    {
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
