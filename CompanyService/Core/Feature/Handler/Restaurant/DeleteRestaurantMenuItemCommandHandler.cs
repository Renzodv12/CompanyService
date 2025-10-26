using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;


using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class DeleteRestaurantMenuItemCommandHandler : IRequestHandler<DeleteRestaurantMenuItemCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRestaurantMenuItemCommandHandler> _logger;

        public DeleteRestaurantMenuItemCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRestaurantMenuItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRestaurantMenuItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting restaurant menu item: {ItemId}", request.Id);

            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.Id == request.Id && mi.MenuId == request.MenuId);

            var menuItem = menuItems.FirstOrDefault();
            if (menuItem == null)
            {
                throw new ArgumentException($"Restaurant menu item with ID {request.Id} not found");
            }

            // Check if item is used in any active orders
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id != null); // Check all sales for now

            var saleDetails = await _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>()
                .WhereAsync(sd => sd.ProductId == menuItem.ProductId);

            if (saleDetails.Any())
            {
                throw new InvalidOperationException("Cannot delete menu item that has been used in orders");
            }

            // Delete the menu item
            _unitOfWork.Repository<RestaurantMenuItem>().Remove(menuItem);

            // Optionally delete the product if it's not used elsewhere
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => p.Id == menuItem.ProductId);

            var product = products.FirstOrDefault();
            if (product != null)
            {
                // Check if product is used in other menu items
                var otherMenuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                    .WhereAsync(mi => mi.ProductId == product.Id && mi.Id != menuItem.Id);

                if (!otherMenuItems.Any())
                {
                    _unitOfWork.Repository<CompanyService.Core.Entities.Product>().Remove(product);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant menu item deleted successfully: {ItemId}", request.Id);
            return true;
        }
    }
}
