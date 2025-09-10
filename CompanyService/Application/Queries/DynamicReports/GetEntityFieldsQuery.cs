using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Queries.DynamicReports
{
    public class GetEntityFieldsQuery : IRequest<IEnumerable<EntityFieldDto>>
    {
        public string EntityName { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}