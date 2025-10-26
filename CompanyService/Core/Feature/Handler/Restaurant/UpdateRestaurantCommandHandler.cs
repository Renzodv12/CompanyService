using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;
using RestaurantEntity = CompanyService.Core.Entities.Restaurant.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, RestaurantDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRestaurantCommandHandler> _logger;

        public UpdateRestaurantCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRestaurantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantDto> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating restaurant: {RestaurantId}", request.Id);

            var restaurants = await _unitOfWork.Repository<RestaurantEntity>()
                .WhereAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId);

            var restaurant = restaurants.FirstOrDefault();
            if (restaurant == null)
            {
                throw new ArgumentException($"Restaurant with ID {request.Id} not found");
            }

            restaurant.Name = request.Name;
            restaurant.Description = request.Description;
            restaurant.Address = request.Address;
            restaurant.City = request.City;
            restaurant.Phone = request.Phone;
            restaurant.Email = request.Email;
            restaurant.RUC = request.RUC;
            restaurant.IsActive = request.IsActive;
            restaurant.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantEntity>().Update(restaurant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant updated successfully: {RestaurantId}", restaurant.Id);

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
                TotalTables = 0, // TODO: Calculate actual values
                AvailableTables = 0,
                TotalMenus = 0,
                ActiveOrders = 0
            };
        }
    }
}
