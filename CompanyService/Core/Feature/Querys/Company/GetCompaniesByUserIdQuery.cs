using CompanyService.Core.Entities;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Company
{
    public record GetCompaniesByUserIdQuery : IRequest<List<CompanyService.Core.Entities.Company>>
    {
        public Guid UserId { get; init; }
    }
}
