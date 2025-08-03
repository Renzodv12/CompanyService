using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Event
{
    public class UpdateAttendeeStatusRequest
    {
        public AttendeeStatus Status { get; set; }
    }
}
