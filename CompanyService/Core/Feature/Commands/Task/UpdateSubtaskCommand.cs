using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class UpdateSubtaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Done { get; set; }
        public Guid TaskId { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
