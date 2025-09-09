using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Menu;
using MediatR;

namespace CompanyService.Core.Handlers.Menu
{
    public class GetMenusByCompanyQuery : IRequest<List<MenuDto>>
    {
        public Guid CompanyId { get; set; }

        public GetMenusByCompanyQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }

    public class GetMenusByCompanyHandler : IRequestHandler<GetMenusByCompanyQuery, List<MenuDto>>
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<GetMenusByCompanyHandler> _logger;

        public GetMenusByCompanyHandler(IMenuService menuService, ILogger<GetMenusByCompanyHandler> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        public async Task<List<MenuDto>> Handle(GetMenusByCompanyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo menús para empresa {CompanyId}", request.CompanyId);
                return await _menuService.GetMenusByCompanyIdAsync(request.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menús para empresa {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}