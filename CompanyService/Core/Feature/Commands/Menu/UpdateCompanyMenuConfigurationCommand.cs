using CompanyService.Core.DTOs.Menu;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Menu
{
    public class UpdateCompanyMenuConfigurationCommand : IRequest<bool>
    {
        public Guid CompanyId { get; set; }
        public List<MenuConfigurationItem> MenuConfigurations { get; set; } = new List<MenuConfigurationItem>();
        
        public UpdateCompanyMenuConfigurationCommand(Guid companyId, List<MenuConfigurationItem> menuConfigurations)
        {
            CompanyId = companyId;
            MenuConfigurations = menuConfigurations;
        }
    }
}
