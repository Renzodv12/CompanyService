using CompanyService.Core.DTOs.Menu;
using CompanyService.Core.Feature.Querys.Menu;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using CompanyService.Core.Services;
using CompanyService.Core.Models.Cache;

namespace CompanyService.Core.Feature.Handler.Menu
{
    public class GetCompanyMenuConfigurationQueryHandler : IRequestHandler<GetCompanyMenuConfigurationQuery, CompanyMenuConfigurationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCompanyMenuConfigurationQueryHandler> _logger;
        private readonly ICacheService _cacheService;

        public GetCompanyMenuConfigurationQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetCompanyMenuConfigurationQueryHandler> logger,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<CompanyMenuConfigurationDto> Handle(GetCompanyMenuConfigurationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting menu configuration for company {CompanyId}", request.CompanyId);

                // Try to get from cache
                var cacheKey = $"menu-config:{request.CompanyId}";
                var cachedResult = await _cacheService.GetAsync<CompanyMenuConfigurationDto>(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for company {CompanyId}", request.CompanyId);
                    return cachedResult;
                }

                _logger.LogInformation("Cache miss for company {CompanyId}, fetching from database", request.CompanyId);

                // Get company information
                var companies = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>()
                    .WhereAsync(c => c.Id == request.CompanyId);
                var company = companies.FirstOrDefault();

                if (company == null)
                {
                    _logger.LogWarning("Company {CompanyId} not found", request.CompanyId);
                    throw new ArgumentException($"Company with ID {request.CompanyId} not found");
                }

                // Get all menus
                var allMenus = await _unitOfWork.Repository<CompanyService.Core.Entities.Menu>()
                    .WhereAsync(m => m.IsActive);
                var menus = allMenus.ToList();

                // Get company menu configurations
                var configurations = await _unitOfWork.Repository<CompanyService.Core.Entities.CompanyMenuConfiguration>()
                    .WhereAsync(cmc => cmc.CompanyId == request.CompanyId);
                var companyConfigurations = configurations.ToList();

                // Create a dictionary for quick lookup
                var configurationDict = companyConfigurations.ToDictionary(c => c.MenuId, c => c.IsEnabled);

                // Build menu hierarchy
                var menuDtos = menus
                    .Where(m => m.ParentId == null) // Only root menus
                    .OrderBy(m => m.Order)
                    .Select(m => MapMenuToDto(m, menus, configurationDict))
                    .ToList();

                var result = new CompanyMenuConfigurationDto
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    Menus = menuDtos
                };

                // Cache the result for 10 minutes
                await _cacheService.SetAsync(cacheKey, result, new CachePolicy 
                { 
                    Expiry = TimeSpan.FromMinutes(10) 
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu configuration for company {CompanyId}", request.CompanyId);
                throw;
            }
        }

        private MenuConfigurationDto MapMenuToDto(CompanyService.Core.Entities.Menu menu, List<CompanyService.Core.Entities.Menu> allMenus, Dictionary<int, bool> configurationDict)
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
