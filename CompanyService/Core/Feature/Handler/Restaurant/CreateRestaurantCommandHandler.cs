using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;
using RestaurantEntity = CompanyService.Core.Entities.Restaurant.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, RestaurantDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRestaurantCommandHandler> _logger;

        public CreateRestaurantCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRestaurantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantDto> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating restaurant: {RestaurantName}", request.Name);

            var restaurant = new RestaurantEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                City = request.City,
                Phone = request.Phone,
                Email = request.Email,
                RUC = request.RUC,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = request.CompanyId,
                CreatedBy = request.UserId
            };

            await _unitOfWork.Repository<RestaurantEntity>().AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant created successfully with ID: {RestaurantId}", restaurant.Id);

            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                City = restaurant.City,
                Phone = restaurant.Phone,
                Email = restaurant.Email,
                RUC = restaurant.RUC,
                IsActive = restaurant.IsActive,
                CreatedAt = restaurant.CreatedAt,
                UpdatedAt = restaurant.UpdatedAt,
                CompanyId = restaurant.CompanyId,
                CreatedBy = restaurant.CreatedBy,
                CreatedByName = "System", // TODO: Get actual user name
                TotalTables = 0,
                AvailableTables = 0,
                TotalMenus = 0,
                ActiveOrders = 0
            };
        }
    }
}
