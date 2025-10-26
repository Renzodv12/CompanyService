using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, RestaurantPaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPaymentCommandHandler> _logger;

        public ProcessPaymentCommandHandler(IUnitOfWork unitOfWork, ILogger<ProcessPaymentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantPaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing payment for order: {OrderId}", request.OrderId);

            // Verify order exists
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.OrderId && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {request.OrderId} not found");
            }

            // Get the sale
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == order.SaleId);

            var sale = sales.FirstOrDefault();
            if (sale == null)
            {
                throw new ArgumentException($"Sale for order {request.OrderId} not found");
            }

            // Check if payment amount matches order total
            if (request.Amount != sale.TotalAmount)
            {
                throw new InvalidOperationException($"Payment amount {request.Amount} does not match order total {sale.TotalAmount}");
            }

            // Update sale with payment information
            sale.PaymentMethod = (CompanyService.Core.Enums.PaymentMethod)request.Method;

            // Update order status to completed
            order.Status = OrderStatus.Completed;
            order.CompletedTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

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

            _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Update(sale);
            _unitOfWork.Repository<RestaurantOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Payment processed successfully for order: {OrderId}", request.OrderId);

            return new RestaurantPaymentDto
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Method = ((PaymentMethod)request.Method).ToString(),
                Status = "Completed",
                TransactionId = request.TransactionId,
                Notes = request.Notes,
                PaymentTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                ProcessedBy = request.UserId,
                ProcessedByName = "System" // TODO: Get actual user name
            };
        }
    }
}
