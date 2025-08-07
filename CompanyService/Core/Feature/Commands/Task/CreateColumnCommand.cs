using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class CreateColumnCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
