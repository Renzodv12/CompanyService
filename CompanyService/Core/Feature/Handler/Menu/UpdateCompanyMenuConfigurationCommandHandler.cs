using CompanyService.Core.Feature.Commands.Menu;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using CompanyService.Core.Services;

namespace CompanyService.Core.Feature.Handler.Menu
{
    public class UpdateCompanyMenuConfigurationCommandHandler : IRequestHandler<UpdateCompanyMenuConfigurationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCompanyMenuConfigurationCommandHandler> _logger;
        private readonly ICacheService _cacheService;

        public UpdateCompanyMenuConfigurationCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateCompanyMenuConfigurationCommandHandler> logger,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(UpdateCompanyMenuConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating menu configuration for company {CompanyId}", request.CompanyId);

                // Verify company exists
                var companies = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>()
                    .WhereAsync(c => c.Id == request.CompanyId);
                var company = companies.FirstOrDefault();

                if (company == null)
                {
                    _logger.LogWarning("Company {CompanyId} not found", request.CompanyId);
                    throw new ArgumentException($"Company with ID {request.CompanyId} not found");
                }

                // Verify all menu IDs exist
                var allMenus = await _unitOfWork.Repository<CompanyService.Core.Entities.Menu>()
                    .WhereAsync(m => m.IsActive);
                var validMenuIds = allMenus.Select(m => m.Id).ToHashSet();

                foreach (var menuConfig in request.MenuConfigurations)
                {
                    if (!validMenuIds.Contains(menuConfig.MenuId))
                    {
                        _logger.LogWarning("Menu {MenuId} does not exist", menuConfig.MenuId);
                        throw new ArgumentException($"Menu with ID {menuConfig.MenuId} does not exist");
                    }
                }

                // Get existing configurations
                var existingConfigurations = await _unitOfWork.Repository<CompanyService.Core.Entities.CompanyMenuConfiguration>()
                    .WhereAsync(cmc => cmc.CompanyId == request.CompanyId);
                var existingConfigs = existingConfigurations.ToList();

                // Create a dictionary for quick lookup
                var existingConfigDict = existingConfigs.ToDictionary(c => c.MenuId, c => c);

                // Process each menu configuration
                foreach (var menuConfig in request.MenuConfigurations)
                {
                    if (existingConfigDict.TryGetValue(menuConfig.MenuId, out var existingConfig))
                    {
                        // Update existing configuration
                        existingConfig.IsEnabled = menuConfig.IsEnabled;
                        existingConfig.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<CompanyService.Core.Entities.CompanyMenuConfiguration>().Update(existingConfig);
                    }
                    else
                    {
                        // Create new configuration
                        var newConfig = new CompanyService.Core.Entities.CompanyMenuConfiguration
                        {
                            CompanyId = request.CompanyId,
                            MenuId = menuConfig.MenuId,
                            IsEnabled = menuConfig.IsEnabled,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.Repository<CompanyService.Core.Entities.CompanyMenuConfiguration>().AddAsync(newConfig);
                    }
                }

                // Save changes
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache for this company's menu configuration
                var cacheKey = $"menu-config:{request.CompanyId}";
                await _cacheService.RemoveAsync(cacheKey);
                _logger.LogInformation("Invalidated cache for company {CompanyId}", request.CompanyId);

                _logger.LogInformation("Successfully updated menu configuration for company {CompanyId}", request.CompanyId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu configuration for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}
