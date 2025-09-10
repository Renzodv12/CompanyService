using MediatR;

namespace CompanyService.Application.Queries.DynamicReports
{
    public class GetAvailableEntitiesQuery : IRequest<IEnumerable<string>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}