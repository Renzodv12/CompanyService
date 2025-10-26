using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class CreateRestaurantOrderCommandHandler : IRequestHandler<CreateRestaurantOrderCommand, RestaurantOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRestaurantOrderCommandHandler> _logger;
        private readonly IRestaurantService _restaurantService;

        public CreateRestaurantOrderCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<CreateRestaurantOrderCommandHandler> logger,
            IRestaurantService restaurantService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _restaurantService = restaurantService;
        }

        public async Task<RestaurantOrderDto> Handle(CreateRestaurantOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating restaurant order for table {TableId}", request.TableId);

            // Convert command to service request
            var serviceRequest = new CompanyService.Core.Services.CreateRestaurantOrderRequest
            {
                RestaurantId = request.RestaurantId,
                TableId = request.TableId,
                CustomerId = null, // Not available in command
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                NumberOfGuests = request.NumberOfGuests,
                Type = (OrderType)request.Type,
                Notes = request.Notes,
                SpecialInstructions = request.SpecialInstructions,
                AssignedWaiterId = null, // Not available in command
                CompanyId = request.CompanyId,
                UserId = request.UserId,
                MenuItems = request.OrderItems.Select(item => new RestaurantOrderMenuItemRequest
                {
                    ProductId = Guid.Empty, // TODO: Get from MenuItem
                    Quantity = item.Quantity,
                    UnitPrice = 0, // TODO: Get from MenuItem
                    Notes = item.SpecialInstructions
                }).ToList()
            };

            // Create the order using the service
            var order = await _restaurantService.CreateRestaurantOrderAsync(serviceRequest);

            _logger.LogInformation("Restaurant order created successfully with ID: {OrderId}", order.Id);

            // Get table information
            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == order.TableId);
            var table = tables.FirstOrDefault();

            // Get sale information
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == order.SaleId);
            var sale = sales.FirstOrDefault();

            return new RestaurantOrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                Type = order.Type.ToString(),
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                NumberOfGuests = order.NumberOfGuests,
                SubTotal = sale?.Subtotal ?? 0,
                TaxAmount = sale?.TaxAmount ?? 0,
                ServiceCharge = 0, // Sale entity doesn't have ServiceCharge
                DiscountAmount = sale?.DiscountAmount ?? 0,
                TotalAmount = sale?.TotalAmount ?? 0,
                Notes = order.Notes,
                SpecialInstructions = order.SpecialInstructions,
                OrderTime = order.OrderTime,
                EstimatedReadyTime = order.EstimatedReadyTime,
                CompletedTime = order.CompletedTime,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                RestaurantId = order.RestaurantId,
                RestaurantName = "Restaurant", // TODO: Get actual restaurant name
                TableId = order.TableId,
                TableNumber = table?.TableNumber ?? "Unknown",
                CreatedBy = order.CreatedBy,
                CreatedByName = "System", // TODO: Get actual user name
                AssignedWaiterId = order.AssignedWaiterId,
                AssignedWaiterName = null,
                OrderItems = new List<RestaurantOrderItemDto>(),
                Payments = new List<RestaurantPaymentDto>()
            };
        }
    }
}
