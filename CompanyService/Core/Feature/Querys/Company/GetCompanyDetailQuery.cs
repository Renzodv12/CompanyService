using CompanyService.Core.Models.Company;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Company
{
    public record GetCompanyDetailQuery : IRequest<CompanyWithPermissionsDto?>
    {
        public Guid UserId { get; init; }
        public Guid CompanyId { get; init; }
    }
}
