using CompanyService.Core.DTOs.Menu;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Menu
{
    public class GetCompanyMenuConfigurationQuery : IRequest<CompanyMenuConfigurationDto>
    {
        public Guid CompanyId { get; set; }
        
        public GetCompanyMenuConfigurationQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
