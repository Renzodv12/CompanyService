using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Menu;
using CompanyService.Core.DTOs.Menu;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CompanyService.Core.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private readonly ILogger<MenuService> _logger;
        private const string MENU_CACHE_KEY_PREFIX = "company_menus:";
        private const int CACHE_EXPIRATION_HOURS = 24;

        public MenuService(IUnitOfWork unitOfWork, IRedisService redisService, ILogger<MenuService> logger)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<List<MenuDto>> GetAllMenusAsync()
        {
            try
            {
                var allMenus = await _unitOfWork.Repository<Menu>().GetAllAsync();
                var activeMenus = allMenus.Where(m => m.IsActive).OrderBy(m => m.Order).ToList();

                return BuildMenuHierarchy(activeMenus.Where(m => m.ParentId == null).ToList(), activeMenus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los menús");
                throw;
            }
        }

        public async Task<List<MenuDto>> GetMenusByCompanyIdAsync(Guid companyId)
        {
            try
            {
                var cacheKey = $"{MENU_CACHE_KEY_PREFIX}{companyId}";
                
                // Intentar obtener del caché
                var cachedMenus = await _redisService.GetAsync<string>(cacheKey);
                if (!string.IsNullOrEmpty(cachedMenus))
                {
                    var deserializedMenus = JsonSerializer.Deserialize<List<MenuDto>>(cachedMenus);
                    if (deserializedMenus != null)
                    {
                        _logger.LogInformation("Menús obtenidos del caché para empresa {CompanyId}", companyId);
                        return deserializedMenus;
                    }
                }

                // Si no está en caché, obtener de la base de datos
                var companyMenus = await GetMenusFromDatabaseAsync(companyId);
                
                // Guardar en caché
                var serializedMenus = JsonSerializer.Serialize(companyMenus);
                await _redisService.SetAsync(cacheKey, serializedMenus, TimeSpan.FromHours(CACHE_EXPIRATION_HOURS));
                
                _logger.LogInformation("Menús obtenidos de la base de datos y guardados en caché para empresa {CompanyId}", companyId);
                return companyMenus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menús para empresa {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<CompanyMenuConfigurationDto> GetCompanyMenuConfigurationAsync(Guid companyId)
        {
            try
            {
                // Get company information
                var companies = await _unitOfWork.Repository<Company>().WhereAsync(c => c.Id == companyId);
                var company = companies.FirstOrDefault();

                if (company == null)
                {
                    throw new ArgumentException($"Company with ID {companyId} not found");
                }

                // Get all menus
                var allMenus = await _unitOfWork.Repository<Menu>().WhereAsync(m => m.IsActive);
                var menus = allMenus.ToList();

                // Get company menu configurations
                var configurations = await _unitOfWork.Repository<CompanyMenuConfiguration>().WhereAsync(cmc => cmc.CompanyId == companyId);
                var companyConfigurations = configurations.ToList();

                // Create a dictionary for quick lookup
                var configurationDict = companyConfigurations.ToDictionary(c => c.MenuId, c => c.IsEnabled);

                // Build menu hierarchy
                var menuDtos = menus
                    .Where(m => m.ParentId == null) // Only root menus
                    .OrderBy(m => m.Order)
                    .Select(m => MapMenuToDto(m, menus, configurationDict))
                    .ToList();

                return new CompanyMenuConfigurationDto
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    Menus = menuDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuración de menús para empresa {CompanyId}", companyId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task UpdateCompanyMenuConfigurationAsync(UpdateMenuConfigurationRequest request)
        {
            try
            {
                // Eliminar configuraciones existentes
                var allConfigurations = await _unitOfWork.Repository<CompanyMenuConfiguration>().GetAllAsync();
                var existingConfigurations = allConfigurations.Where(cmc => cmc.CompanyId == request.CompanyId).ToList();

                foreach (var config in existingConfigurations)
                {
                    _unitOfWork.Repository<CompanyMenuConfiguration>().Remove(config);
                }

                // Agregar nuevas configuraciones
                foreach (var menuConfig in request.MenuConfigurations)
                {
                    var newConfig = new CompanyMenuConfiguration
                    {
                        CompanyId = request.CompanyId,
                        MenuId = menuConfig.MenuId,
                        IsEnabled = menuConfig.IsEnabled,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    await _unitOfWork.Repository<CompanyMenuConfiguration>().AddAsync(newConfig);
                }

                await _unitOfWork.SaveChangesAsync();
                
                // Invalidar caché
                await InvalidateMenuCacheAsync(request.CompanyId);
                
                _logger.LogInformation("Configuración de menús actualizada para empresa {CompanyId}", request.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuración de menús para empresa {CompanyId}", request.CompanyId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task InvalidateMenuCacheAsync(Guid companyId)
        {
            try
            {
                var cacheKey = $"{MENU_CACHE_KEY_PREFIX}{companyId}";
                await _redisService.DeleteAsync(cacheKey);
                _logger.LogInformation("Caché de menús invalidado para empresa {CompanyId}", companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invalidar caché de menús para empresa {CompanyId}", companyId);
            }
        }

        private async Task<List<MenuDto>> GetMenusFromDatabaseAsync(Guid companyId)
        {
            // Verificar si la empresa tiene configuración personalizada
            var allConfigurations = await _unitOfWork.Repository<CompanyMenuConfiguration>().GetAllAsync();
            var hasCustomConfiguration = allConfigurations.Any(cmc => cmc.CompanyId == companyId);

            if (hasCustomConfiguration)
            {
                // Obtener menús configurados para la empresa
                var allMenuConfigs = await _unitOfWork.Repository<CompanyMenuConfiguration>().GetAllAsync();
                var companyMenuConfigs = allMenuConfigs.Where(cmc => cmc.CompanyId == companyId && cmc.IsEnabled).ToList();

                var menuIds = companyMenuConfigs.Select(cmc => cmc.MenuId).ToList();
                var allMenus = await _unitOfWork.Repository<Menu>().GetAllAsync();
                var configuredMenus = allMenus
                    .Where(m => menuIds.Contains(m.Id) && m.IsActive)
                    .OrderBy(m => m.Order)
                    .ToList();

                return BuildMenuHierarchy(configuredMenus.Where(m => m.ParentId == null).ToList(), configuredMenus);
            }
            else
            {
                // Devolver todos los menús activos (comportamiento por defecto)
                return await GetAllMenusAsync();
            }
        }

        private List<MenuDto> BuildMenuHierarchy(List<Menu> parentMenus, List<Menu>? allMenus = null)
        {
            var result = new List<MenuDto>();
            var menusToUse = allMenus ?? parentMenus;

            foreach (var menu in parentMenus.OrderBy(m => m.Order))
            {
                var menuDto = MapToMenuDto(menu);
                
                var children = menusToUse.Where(m => m.ParentId == menu.Id && m.IsActive).ToList();
                if (children.Any())
                {
                    menuDto.Children = BuildMenuHierarchy(children, menusToUse);
                }
                
                result.Add(menuDto);
            }

            return result;
        }

        private MenuDto MapToMenuDto(Menu menu)
        {
            return new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Icon = menu.Icon,
                Route = menu.Route,
                Description = menu.Description,
                ParentId = menu.ParentId,
                Order = menu.Order,
                IsActive = menu.IsActive
            };
        }

        private MenuConfigurationDto MapMenuToDto(Menu menu, List<Menu> allMenus, Dictionary<int, bool> configurationDict)
        {
            var dto = new MenuConfigurationDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Icon = menu.Icon,
                Route = menu.Route,
                Description = menu.Description,
                ParentId = menu.ParentId,
                Order = menu.Order,
                IsActive = menu.IsActive,
                IsEnabled = configurationDict.GetValueOrDefault(menu.Id, true) // Default to enabled if not configured
            };

            // Add children
            var children = allMenus
                .Where(m => m.ParentId == menu.Id)
                .OrderBy(m => m.Order)
                .Select(m => MapMenuToDto(m, allMenus, configurationDict))
                .ToList();

            dto.Children = children;

            return dto;
        }
    }
}