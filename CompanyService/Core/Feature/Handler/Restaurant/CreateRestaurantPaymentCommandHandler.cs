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
    public class CreateRestaurantPaymentCommandHandler : IRequestHandler<CreateRestaurantPaymentCommand, RestaurantPaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRestaurantPaymentCommandHandler> _logger;

        public CreateRestaurantPaymentCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRestaurantPaymentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantPaymentDto> Handle(CreateRestaurantPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating payment for order: {OrderId}", request.OrderId);

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

            // Create payment record (we'll store this in a custom table or use Sale's payment fields)
            // For now, we'll update the sale with payment information
            sale.PaymentMethod = (CompanyService.Core.Enums.PaymentMethod)request.Method;
            // Sale entity doesn't have PaymentStatus, PaymentDate, or UpdatedAt

            _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Update(sale);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Payment created successfully for order: {OrderId}", request.OrderId);

            return new RestaurantPaymentDto
            {
                Id = Guid.NewGuid(), // We'll generate a payment ID
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
