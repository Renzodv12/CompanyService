using CompanyService.Core.Models.Event;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Event
{
    public class GetEventByIdQuery : IRequest<EventDto?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
