using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    // Restaurant Order Commands
    public class CreateRestaurantOrderCommand : IRequest<RestaurantOrderDto>
    {
        public Guid TableId { get; set; }
        public int Type { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public int NumberOfGuests { get; set; } = 1;
        public string? Notes { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<CreateOrderItemCommand> OrderItems { get; set; } = new();
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateOrderItemCommand
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
    }

    public class UpdateRestaurantOrderCommand : IRequest<RestaurantOrderDto>
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public int NumberOfGuests { get; set; }
        public string? Notes { get; set; }
        public string? SpecialInstructions { get; set; }
        public Guid? AssignedWaiterId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteRestaurantOrderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class AddOrderItemCommand : IRequest<RestaurantOrderItemDto>
    {
        public Guid OrderId { get; set; }
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class UpdateOrderItemCommand : IRequest<RestaurantOrderItemDto>
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
        public int Status { get; set; }
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class RemoveOrderItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}

