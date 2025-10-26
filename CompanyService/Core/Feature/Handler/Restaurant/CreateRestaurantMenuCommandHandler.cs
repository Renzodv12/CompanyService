using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class CreateRestaurantMenuCommandHandler : IRequestHandler<CreateRestaurantMenuCommand, RestaurantMenuDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRestaurantMenuCommandHandler> _logger;

        public CreateRestaurantMenuCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRestaurantMenuCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantMenuDto> Handle(CreateRestaurantMenuCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating restaurant menu: {MenuName}", request.Name);

            var menu = new RestaurantMenu
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RestaurantId = request.RestaurantId,
                CompanyId = request.CompanyId,
                CreatedBy = request.UserId
            };

            await _unitOfWork.Repository<RestaurantMenu>().AddAsync(menu);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant menu created successfully with ID: {MenuId}", menu.Id);

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
                TotalItems = 0,
                AvailableItems = 0
            };
        }
    }
}

