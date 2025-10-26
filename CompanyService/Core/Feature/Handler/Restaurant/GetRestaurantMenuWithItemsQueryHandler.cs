using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantMenuWithItemsQueryHandler : IRequestHandler<GetRestaurantMenuWithItemsQuery, RestaurantMenuWithItemsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantMenuWithItemsQueryHandler> _logger;

        public GetRestaurantMenuWithItemsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantMenuWithItemsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantMenuWithItemsDto?> Handle(GetRestaurantMenuWithItemsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting menu with items for restaurant {RestaurantId}", request.RestaurantId);

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

            var query = menuItems.AsQueryable();

            if (request.Category != null)
            {
                query = query.Where(mi => mi.Category == request.Category);
            }

            if (request.IsAvailable.HasValue)
            {
                query = query.Where(mi => mi.IsAvailable == request.IsAvailable.Value);
            }

            if (request.IsVegetarian.HasValue)
            {
                query = query.Where(mi => mi.IsVegetarian == request.IsVegetarian.Value);
            }

            if (request.IsVegan.HasValue)
            {
                query = query.Where(mi => mi.IsVegan == request.IsVegan.Value);
            }

            if (request.IsGlutenFree.HasValue)
            {
                query = query.Where(mi => mi.IsGlutenFree == request.IsGlutenFree.Value);
            }

            var items = query
                .OrderBy(mi => mi.Category)
                .ThenBy(mi => mi.ProductId) // We'll need to join with Product to get the name
                .Select(mi => new RestaurantMenuItemDto
                {
                    Id = mi.Id,
                    Name = "Product", // TODO: Get actual product name
                    Description = "Description", // TODO: Get actual product description
                    Price = 0, // TODO: Get actual product price
                    Category = mi.Category,
                    ImageUrl = mi.ImageUrl,
                    IsAvailable = mi.IsAvailable,
                    PreparationTime = mi.PreparationTime,
                    Allergens = mi.Allergens,
                    IsVegetarian = mi.IsVegetarian,
                    IsVegan = mi.IsVegan,
                    IsGlutenFree = mi.IsGlutenFree,
                    CreatedAt = mi.CreatedAt,
                    UpdatedAt = mi.UpdatedAt,
                    MenuId = mi.MenuId,
                    MenuName = menu.Name
                })
                .ToList();

            return new RestaurantMenuWithItemsDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Description = menu.Description,
                IsActive = menu.IsActive,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.UpdatedAt,
                RestaurantId = menu.RestaurantId,
                RestaurantName = "Restaurant", // TODO: Get actual restaurant name
                TotalItems = items.Count,
                AvailableItems = items.Count(i => i.IsAvailable),
                Items = items
            };
        }
    }
}
