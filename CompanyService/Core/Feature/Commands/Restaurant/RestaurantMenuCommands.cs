using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    // Restaurant Menu Commands
    public class CreateRestaurantMenuCommand : IRequest<RestaurantMenuDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class UpdateRestaurantMenuCommand : IRequest<RestaurantMenuDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteRestaurantMenuCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    // Restaurant Menu Item Commands
    public class CreateRestaurantMenuItemCommand : IRequest<RestaurantMenuItemDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTime { get; set; } = 15;
        public string? Allergens { get; set; }
        public bool IsVegetarian { get; set; } = false;
        public bool IsVegan { get; set; } = false;
        public bool IsGlutenFree { get; set; } = false;
        public Guid MenuId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class UpdateRestaurantMenuItemCommand : IRequest<RestaurantMenuItemDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTime { get; set; }
        public string? Allergens { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsAvailable { get; set; }
        public Guid MenuId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteRestaurantMenuItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}

