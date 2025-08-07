using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class UpdateTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Labels { get; set; } = new();
        public List<string> AssigneeUserIds { get; set; } = new();
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
