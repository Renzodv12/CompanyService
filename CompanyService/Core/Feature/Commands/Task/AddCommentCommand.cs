using MediatR;

namespace CompanyService.Core.Feature.Commands.Task
{
    public class AddCommentCommand : IRequest<Guid>
    {
        public Guid TaskId { get; set; }
        public string Content { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }

}
