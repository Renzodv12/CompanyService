using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; } = false;
        public EventPriority Priority { get; set; } = EventPriority.Low;
        public bool IsActive { get; set; } = true;
        public Guid CompanyId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Company Company { get; set; }
        public ICollection<EventAttendee> EventAttendees { get; set; } = new List<EventAttendee>();
    }
}