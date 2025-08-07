using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class ClearColumnCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
