using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class DragTaskCommand : IRequest<bool>
    {
        public Guid TaskId { get; set; }
        public Guid SourceColumnId { get; set; }
        public Guid TargetColumnId { get; set; }
        public int NewPosition { get; set; }
        public Guid? TargetTaskId { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
