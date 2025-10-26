using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class DeleteRestaurantMenuCommandHandler : IRequestHandler<DeleteRestaurantMenuCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRestaurantMenuCommandHandler> _logger;

        public DeleteRestaurantMenuCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRestaurantMenuCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRestaurantMenuCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting restaurant menu: {MenuId}", request.Id);

            var menus = await _unitOfWork.Repository<RestaurantMenu>()
                .WhereAsync(m => m.Id == request.Id && m.RestaurantId == request.RestaurantId);

            var menu = menus.FirstOrDefault();
            if (menu == null)
            {
                throw new ArgumentException($"Restaurant menu with ID {request.Id} not found");
            }

            // Check if menu has items
            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.MenuId == request.Id);

            if (menuItems.Any())
            {
                throw new InvalidOperationException("Cannot delete menu with existing items");
            }

            _unitOfWork.Repository<RestaurantMenu>().Remove(menu);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant menu deleted successfully: {MenuId}", request.Id);
            return true;
        }
    }
}

