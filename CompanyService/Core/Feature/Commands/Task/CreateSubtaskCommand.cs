using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class CreateSubtaskCommand : IRequest<Guid>
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
