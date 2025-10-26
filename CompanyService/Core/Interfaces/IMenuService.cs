using CompanyService.Core.Models.Menu;
using CompanyService.Core.DTOs.Menu;

namespace CompanyService.Core.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetAllMenusAsync();
        Task<List<MenuDto>> GetMenusByCompanyIdAsync(Guid companyId);
        Task<CompanyMenuConfigurationDto> GetCompanyMenuConfigurationAsync(Guid companyId);
        System.Threading.Tasks.Task UpdateCompanyMenuConfigurationAsync(UpdateMenuConfigurationRequest request);
        System.Threading.Tasks.Task InvalidateMenuCacheAsync(Guid companyId);
    }
}