using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Menu;
using MediatR;

namespace CompanyService.Core.Handlers.Menu
{
    public class GetCompanyMenuConfigurationQuery : IRequest<List<CompanyMenuConfigurationDto>>
    {
        public Guid CompanyId { get; set; }

        public GetCompanyMenuConfigurationQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }

    public class GetCompanyMenuConfigurationHandler : IRequestHandler<GetCompanyMenuConfigurationQuery, List<CompanyMenuConfigurationDto>>
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<GetCompanyMenuConfigurationHandler> _logger;

        public GetCompanyMenuConfigurationHandler(IMenuService menuService, ILogger<GetCompanyMenuConfigurationHandler> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        public async Task<List<CompanyMenuConfigurationDto>> Handle(GetCompanyMenuConfigurationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo configuración de menús para empresa {CompanyId}", request.CompanyId);
                return await _menuService.GetCompanyMenuConfigurationAsync(request.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuración de menús para empresa {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}