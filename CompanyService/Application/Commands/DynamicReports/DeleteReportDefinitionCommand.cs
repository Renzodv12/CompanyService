using MediatR;

namespace CompanyService.Application.Commands.DynamicReports
{
    public class DeleteReportDefinitionCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}