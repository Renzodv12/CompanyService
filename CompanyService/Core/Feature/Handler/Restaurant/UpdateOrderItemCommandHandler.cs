using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;


using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class UpdateOrderItemCommandHandler : IRequestHandler<UpdateOrderItemCommand, RestaurantOrderItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateOrderItemCommandHandler> _logger;

        public UpdateOrderItemCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateOrderItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantOrderItemDto> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating order item: {ItemId}", request.Id);

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
                throw new InvalidOperationException("Cannot update items in completed or cancelled orders");
            }

            // Update sale detail
            var oldTotal = saleDetail.Subtotal;
            saleDetail.Quantity = request.Quantity;
            saleDetail.Subtotal = saleDetail.UnitPrice * request.Quantity;

            // Update sale totals
            sale.Subtotal = sale.Subtotal - oldTotal + saleDetail.Subtotal;
            sale.TaxAmount = sale.Subtotal * 0.18m; // 18% tax
            sale.TotalAmount = sale.Subtotal + sale.TaxAmount - sale.DiscountAmount;

            _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>().Update(saleDetail);
            _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Update(sale);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order item updated successfully: {ItemId}", request.Id);

            // Get menu item info
            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.ProductId == saleDetail.ProductId);

            var menuItem = menuItems.FirstOrDefault();

            return new RestaurantOrderItemDto
            {
                Id = saleDetail.Id,
                Quantity = saleDetail.Quantity,
                UnitPrice = saleDetail.UnitPrice,
                TotalPrice = saleDetail.Subtotal,
                SpecialInstructions = request.SpecialInstructions,
                Status = "Updated",
                CreatedAt = DateTime.UtcNow, // SaleDetail doesn't have CreatedAt
                UpdatedAt = DateTime.UtcNow, // SaleDetail doesn't have UpdatedAt
                OrderId = order.Id,
                MenuItemId = menuItem?.Id ?? Guid.Empty,
                MenuItemName = "Product", // TODO: Get actual product name
                MenuItemCategory = menuItem?.Category
            };
        }
    }
}
