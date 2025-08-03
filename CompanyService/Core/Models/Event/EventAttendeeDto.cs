using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Event
{
    public class EventAttendeeDto
    {
        public Guid UserId { get; set; }
        public AttendeeStatus Status { get; set; }
        public bool IsOrganizer { get; set; }
    }

}
