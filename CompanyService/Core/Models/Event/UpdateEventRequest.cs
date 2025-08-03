using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Event
{
    public class UpdateEventRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        public EventPriority Priority { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> AttendeeUserIds { get; set; } = new();
    }

}
