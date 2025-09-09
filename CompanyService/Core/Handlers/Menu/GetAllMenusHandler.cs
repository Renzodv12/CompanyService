using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Menu;
using MediatR;

namespace CompanyService.Core.Handlers.Menu
{
    public class GetAllMenusQuery : IRequest<List<MenuDto>>
    {
    }

    public class GetAllMenusHandler : IRequestHandler<GetAllMenusQuery, List<MenuDto>>
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<GetAllMenusHandler> _logger;

        public GetAllMenusHandler(IMenuService menuService, ILogger<GetAllMenusHandler> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        public async Task<List<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los menús disponibles");
                return await _menuService.GetAllMenusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los menús");
                throw;
            }
        }
    }
}