using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class EventAttendee
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public AttendeeStatus Status { get; set; } = AttendeeStatus.Pending;
        public bool IsOrganizer { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Event Event { get; set; }
    }
}