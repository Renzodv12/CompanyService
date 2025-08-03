using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Event
{
    public class UpdateAttendeeStatusCommand : IRequest<bool>
    {
        public Guid EventId { get; set; }
        public AttendeeStatus Status { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
