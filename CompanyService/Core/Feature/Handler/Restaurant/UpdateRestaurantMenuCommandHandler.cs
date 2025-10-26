using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class UpdateRestaurantMenuCommandHandler : IRequestHandler<UpdateRestaurantMenuCommand, RestaurantMenuDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRestaurantMenuCommandHandler> _logger;

        public UpdateRestaurantMenuCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRestaurantMenuCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantMenuDto> Handle(UpdateRestaurantMenuCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating restaurant menu: {MenuId}", request.Id);

            var menus = await _unitOfWork.Repository<RestaurantMenu>()
                .WhereAsync(m => m.Id == request.Id && m.RestaurantId == request.RestaurantId);

            var menu = menus.FirstOrDefault();
            if (menu == null)
            {
                throw new ArgumentException($"Restaurant menu with ID {request.Id} not found");
            }

            menu.Name = request.Name;
            menu.Description = request.Description;
            menu.IsActive = request.IsActive;
            menu.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantMenu>().Update(menu);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant menu updated successfully: {MenuId}", menu.Id);

            return new RestaurantMenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Description = menu.Description,
                IsActive = menu.IsActive,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.UpdatedAt,
                RestaurantId = menu.RestaurantId,
                RestaurantName = "Restaurant", // TODO: Get actual restaurant name
                TotalItems = 0, // TODO: Calculate actual values
                AvailableItems = 0
            };
        }
    }
}

