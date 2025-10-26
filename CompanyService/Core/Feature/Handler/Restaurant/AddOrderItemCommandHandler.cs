using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, RestaurantOrderItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddOrderItemCommandHandler> _logger;

        public AddOrderItemCommandHandler(IUnitOfWork unitOfWork, ILogger<AddOrderItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantOrderItemDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding item to order: {OrderId}", request.OrderId);

            // Verify order exists and is in correct status
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.OrderId && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {request.OrderId} not found");
            }

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot add items to completed or cancelled orders");
            }

            // Get menu item
            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.Id == request.MenuItemId);

            var menuItem = menuItems.FirstOrDefault();
            if (menuItem == null)
            {
                throw new ArgumentException($"Menu item with ID {request.MenuItemId} not found");
            }

            // Get associated product
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => p.Id == menuItem.ProductId);

            var product = products.FirstOrDefault();
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {menuItem.ProductId} not found");
            }

            // Create sale detail
            var saleDetail = new CompanyService.Core.Entities.SaleDetail
            {
                Id = Guid.NewGuid(),
                SaleId = Guid.Empty, // Will be set when we get the sale
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                Subtotal = product.Price * request.Quantity
            };

            // Get the sale associated with this order
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == order.SaleId);

            var sale = sales.FirstOrDefault();
            if (sale != null)
            {
                saleDetail.SaleId = sale.Id;
                await _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>().AddAsync(saleDetail);

                // Update sale totals
                sale.Subtotal += saleDetail.Subtotal;
                sale.TaxAmount = sale.Subtotal * 0.18m; // 18% tax
                sale.TotalAmount = sale.Subtotal + sale.TaxAmount - sale.DiscountAmount;
                _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Update(sale);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item added to order successfully: {OrderId}", request.OrderId);

            return new RestaurantOrderItemDto
            {
                Id = saleDetail.Id,
                Quantity = saleDetail.Quantity,
                UnitPrice = saleDetail.UnitPrice,
                TotalPrice = saleDetail.Subtotal,
                SpecialInstructions = request.SpecialInstructions,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderId = order.Id,
                MenuItemId = menuItem.Id,
                MenuItemName = product.Name,
                MenuItemCategory = menuItem.Category
            };
        }
    }
}