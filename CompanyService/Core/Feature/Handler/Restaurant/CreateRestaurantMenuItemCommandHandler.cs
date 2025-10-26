using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class CreateRestaurantMenuItemCommandHandler : IRequestHandler<CreateRestaurantMenuItemCommand, RestaurantMenuItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRestaurantMenuItemCommandHandler> _logger;

        public CreateRestaurantMenuItemCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRestaurantMenuItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantMenuItemDto> Handle(CreateRestaurantMenuItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating restaurant menu item: {ItemName}", request.Name);

            // Create the product first
            var product = new CompanyService.Core.Entities.Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = Guid.Empty, // TODO: Set proper category ID
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                CompanyId = request.CompanyId
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Product>().AddAsync(product);

            // Create the menu item
            var menuItem = new RestaurantMenuItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                MenuId = request.MenuId,
                RestaurantId = request.RestaurantId,
                Category = request.Category,
                ImageUrl = request.ImageUrl,
                IsAvailable = true,
                PreparationTime = request.PreparationTime,
                Allergens = request.Allergens,
                IsVegetarian = request.IsVegetarian,
                IsVegan = request.IsVegan,
                IsGlutenFree = request.IsGlutenFree,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RestaurantMenuItem>().AddAsync(menuItem);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant menu item created successfully with ID: {ItemId}", menuItem.Id);

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
