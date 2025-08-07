using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class UpdateColumnCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
