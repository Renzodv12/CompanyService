using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantMenuQueryHandler : IRequestHandler<GetRestaurantMenuQuery, RestaurantMenuDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantMenuQueryHandler> _logger;

        public GetRestaurantMenuQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantMenuQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantMenuDto?> Handle(GetRestaurantMenuQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting menu for restaurant {RestaurantId}", request.RestaurantId);

            var menus = await _unitOfWork.Repository<RestaurantMenu>()
                .WhereAsync(m => m.RestaurantId == request.RestaurantId && m.IsActive);

            var menu = menus.FirstOrDefault();
            if (menu == null)
            {
                return null;
            }

            // Get menu items
            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.MenuId == menu.Id);

            var availableItems = menuItems.Count(mi => mi.IsAvailable);

            return new RestaurantMenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Description = menu.Description,
                IsActive = menu.IsActive,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.UpdatedAt,
                RestaurantId = menu.RestaurantId,
                RestaurantName = "Restaurant", // TODO: Get actual restaurant name
                TotalItems = menuItems.Count(),
                AvailableItems = availableItems
            };
        }
    }
}
