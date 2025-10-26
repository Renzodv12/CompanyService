using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantOrderByIdQueryHandler : IRequestHandler<GetRestaurantOrderByIdQuery, RestaurantOrderDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantOrderByIdQueryHandler> _logger;

        public GetRestaurantOrderByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantOrderByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantOrderDto?> Handle(GetRestaurantOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting order {OrderId} for restaurant {RestaurantId}", request.Id, request.RestaurantId);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.Id && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                return null;
            }

            // Get sale information
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == order.SaleId);

            var sale = sales.FirstOrDefault();

            // Get order items (sale details)
            var saleDetails = await _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>()
                .WhereAsync(sd => sd.SaleId == sale.Id);

            var orderItems = saleDetails.Select(sd => new RestaurantOrderItemDto
            {
                Id = sd.Id,
                Quantity = sd.Quantity,
                UnitPrice = sd.UnitPrice,
                TotalPrice = sd.Subtotal,
                SpecialInstructions = null,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderId = order.Id,
                MenuItemId = Guid.Empty, // TODO: Get from RestaurantMenuItem
                MenuItemName = "Product", // TODO: Get actual product name
                MenuItemCategory = null
            }).ToList();

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
                RestaurantName = "Restaurant",
                TableId = order.TableId,
                TableNumber = "Table",
                CreatedBy = order.CreatedBy,
                CreatedByName = "System",
                AssignedWaiterId = order.AssignedWaiterId,
                AssignedWaiterName = null,
                OrderItems = orderItems,
                Payments = new List<RestaurantPaymentDto>()
            };
        }
    }
}
