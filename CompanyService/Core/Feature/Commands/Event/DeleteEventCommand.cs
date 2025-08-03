using MediatR;

namespace CompanyService.Core.Feature.Commands.Event
{
    public class DeleteEventCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
