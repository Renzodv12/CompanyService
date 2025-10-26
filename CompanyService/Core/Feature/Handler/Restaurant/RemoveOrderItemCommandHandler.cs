using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;


using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveOrderItemCommandHandler> _logger;

        public RemoveOrderItemCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveOrderItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing order item: {ItemId}", request.Id);

            // Get sale detail
            var saleDetails = await _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>()
                .WhereAsync(sd => sd.Id == request.Id);

            var saleDetail = saleDetails.FirstOrDefault();
            if (saleDetail == null)
            {
                throw new ArgumentException($"Order item with ID {request.Id} not found");
            }

            // Get the sale
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == saleDetail.SaleId);

            var sale = sales.FirstOrDefault();
            if (sale == null)
            {
                throw new ArgumentException($"Sale with ID {saleDetail.SaleId} not found");
            }

            // Get the order
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.OrderId && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {request.OrderId} not found");
            }

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot remove items from completed or cancelled orders");
            }

            // Update sale totals
            sale.Subtotal -= saleDetail.Subtotal;
            sale.TaxAmount = sale.Subtotal * 0.18m; // 18% tax
            sale.TotalAmount = sale.Subtotal + sale.TaxAmount - sale.DiscountAmount;

            // Remove sale detail
            _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>().Remove(saleDetail);
            _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Update(sale);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order item removed successfully: {ItemId}", request.Id);
            return true;
        }
    }
}
