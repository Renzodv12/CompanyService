using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantPaymentsQueryHandler : IRequestHandler<GetRestaurantPaymentsQuery, List<RestaurantPaymentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantPaymentsQueryHandler> _logger;

        public GetRestaurantPaymentsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantPaymentsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<RestaurantPaymentDto>> Handle(GetRestaurantPaymentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting payments for order {OrderId}", request.OrderId);

            // Get the order
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.OrderId && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                return new List<RestaurantPaymentDto>();
            }

            // Get the sale
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == order.SaleId);

            var sale = sales.FirstOrDefault();
            if (sale == null)
            {
                return new List<RestaurantPaymentDto>();
            }

            // Create payment DTO from sale information
            var payments = new List<RestaurantPaymentDto>();

            // Since Sale entity doesn't have PaymentStatus or PaymentDate, 
            // we'll create a payment if the sale exists
            if (sale != null)
            {
                payments.Add(new RestaurantPaymentDto
                {
                    Id = Guid.NewGuid(), // Generate a payment ID
                    Amount = sale.TotalAmount,
                    Method = sale.PaymentMethod.ToString() ?? "Cash",
                    Status = "Completed", // Assume completed if sale exists
                    TransactionId = null,
                    Notes = null,
                    PaymentTime = sale.CreatedAt, // Use sale creation date as payment date
                    CreatedAt = sale.CreatedAt,
                    UpdatedAt = sale.CreatedAt,
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    ProcessedBy = sale.UserId,
                    ProcessedByName = "System" // TODO: Get actual user name
                });
            }

            return payments;
        }
    }
}
