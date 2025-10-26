using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class UpdateRestaurantOrderCommandHandler : IRequestHandler<UpdateRestaurantOrderCommand, RestaurantOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRestaurantOrderCommandHandler> _logger;

        public UpdateRestaurantOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRestaurantOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantOrderDto> Handle(UpdateRestaurantOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating restaurant order: {OrderId}", request.Id);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.Id && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {request.Id} not found");
            }

            // Update order properties
            order.Status = (OrderStatus)request.Status;
            order.CustomerName = request.CustomerName;
            order.CustomerPhone = request.CustomerPhone;
            order.NumberOfGuests = request.NumberOfGuests;
            order.Notes = request.Notes;
            order.SpecialInstructions = request.SpecialInstructions;
            order.AssignedWaiterId = request.AssignedWaiterId;
            order.UpdatedAt = DateTime.UtcNow;

            // If order is completed, update completion time and table status
            if (order.Status == OrderStatus.Completed)
            {
                order.CompletedTime = DateTime.UtcNow;

                // Update table status to available
                var tables = await _unitOfWork.Repository<RestaurantTable>()
                    .WhereAsync(t => t.Id == order.TableId);

                var table = tables.FirstOrDefault();
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    table.CurrentOrderId = null;
                    table.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<RestaurantTable>().Update(table);
                }
            }

            _unitOfWork.Repository<RestaurantOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant order updated successfully: {OrderId}", order.Id);

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
                TableNumber = "Table", // TODO: Get actual table number
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
