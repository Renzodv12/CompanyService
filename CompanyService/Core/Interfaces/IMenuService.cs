using CompanyService.Core.Models.Menu;

namespace CompanyService.Core.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetAllMenusAsync();
        Task<List<MenuDto>> GetMenusByCompanyIdAsync(Guid companyId);
        Task<List<CompanyMenuConfigurationDto>> GetCompanyMenuConfigurationAsync(Guid companyId);
        System.Threading.Tasks.Task UpdateCompanyMenuConfigurationAsync(UpdateCompanyMenuConfigurationRequest request);
        System.Threading.Tasks.Task InvalidateMenuCacheAsync(Guid companyId);
    }
}