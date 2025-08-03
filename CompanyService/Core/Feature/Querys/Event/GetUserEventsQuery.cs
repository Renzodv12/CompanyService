using CompanyService.Core.Models.Event;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Event
{
    public class GetUserEventsQuery : IRequest<List<EventDto>>
    {
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
