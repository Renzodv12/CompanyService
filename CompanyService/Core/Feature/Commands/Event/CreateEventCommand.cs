using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Event
{
    public class CreateEventCommand : IRequest<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        public EventPriority Priority { get; set; }
        public List<Guid> AttendeeUserIds { get; set; } = new();
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
