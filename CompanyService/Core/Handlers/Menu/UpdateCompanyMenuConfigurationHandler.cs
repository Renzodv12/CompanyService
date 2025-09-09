using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Menu;
using MediatR;

namespace CompanyService.Core.Handlers.Menu
{
    public class UpdateCompanyMenuConfigurationCommand : IRequest<bool>
    {
        public UpdateCompanyMenuConfigurationRequest Request { get; set; }

        public UpdateCompanyMenuConfigurationCommand(UpdateCompanyMenuConfigurationRequest request)
        {
            Request = request;
        }
    }

    public class UpdateCompanyMenuConfigurationHandler : IRequestHandler<UpdateCompanyMenuConfigurationCommand, bool>
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<UpdateCompanyMenuConfigurationHandler> _logger;

        public UpdateCompanyMenuConfigurationHandler(IMenuService menuService, ILogger<UpdateCompanyMenuConfigurationHandler> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateCompanyMenuConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Actualizando configuración de menús para empresa {CompanyId}", request.Request.CompanyId);
                await _menuService.UpdateCompanyMenuConfigurationAsync(request.Request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuración de menús para empresa {CompanyId}", request.Request.CompanyId);
                throw;
            }
        }
    }
}