using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class UpdateRestaurantMenuItemCommandHandler : IRequestHandler<UpdateRestaurantMenuItemCommand, RestaurantMenuItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRestaurantMenuItemCommandHandler> _logger;

        public UpdateRestaurantMenuItemCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRestaurantMenuItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantMenuItemDto> Handle(UpdateRestaurantMenuItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating restaurant menu item: {ItemId}", request.Id);

            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.Id == request.Id && mi.MenuId == request.MenuId);

            var menuItem = menuItems.FirstOrDefault();
            if (menuItem == null)
            {
                throw new ArgumentException($"Restaurant menu item with ID {request.Id} not found");
            }

            // Get the associated product
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => p.Id == menuItem.ProductId);

            var product = products.FirstOrDefault();
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {menuItem.ProductId} not found");
            }

            // Update product
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.LastModifiedAt = DateTime.UtcNow;

            // Update menu item
            menuItem.Category = request.Category;
            menuItem.ImageUrl = request.ImageUrl;
            menuItem.IsAvailable = request.IsAvailable;
            menuItem.PreparationTime = request.PreparationTime;
            menuItem.Allergens = request.Allergens;
            menuItem.IsVegetarian = request.IsVegetarian;
            menuItem.IsVegan = request.IsVegan;
            menuItem.IsGlutenFree = request.IsGlutenFree;
            menuItem.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CompanyService.Core.Entities.Product>().Update(product);
            _unitOfWork.Repository<RestaurantMenuItem>().Update(menuItem);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant menu item updated successfully: {ItemId}", menuItem.Id);

            return new RestaurantMenuItemDto
            {
                Id = menuItem.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = menuItem.Category,
                ImageUrl = menuItem.ImageUrl,
                IsAvailable = menuItem.IsAvailable,
                PreparationTime = menuItem.PreparationTime,
                Allergens = menuItem.Allergens,
                IsVegetarian = menuItem.IsVegetarian,
                IsVegan = menuItem.IsVegan,
                IsGlutenFree = menuItem.IsGlutenFree,
                CreatedAt = menuItem.CreatedAt,
                UpdatedAt = menuItem.UpdatedAt,
                MenuId = menuItem.MenuId,
                MenuName = "Menu" // TODO: Get actual menu name
            };
        }
    }
}
