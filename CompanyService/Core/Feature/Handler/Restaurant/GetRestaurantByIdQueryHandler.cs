using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;
using RestaurantEntity = CompanyService.Core.Entities.Restaurant.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantByIdQueryHandler> _logger;

        public GetRestaurantByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantDto?> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting restaurant by ID: {RestaurantId}", request.Id);

            var restaurants = await _unitOfWork.Repository<RestaurantEntity>()
                .WhereAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId);

            var restaurant = restaurants.FirstOrDefault();
            if (restaurant == null)
            {
                _logger.LogWarning("Restaurant not found: {RestaurantId}", request.Id);
                return null;
            }

            // Calculate statistics
            var totalTables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.RestaurantId == request.Id);

            var availableTables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.RestaurantId == request.Id && t.Status == TableStatus.Available);

            var totalMenus = await _unitOfWork.Repository<RestaurantMenu>()
                .WhereAsync(m => m.RestaurantId == request.Id && m.IsActive);

            var activeOrders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == request.Id && 
                               (o.Status == OrderStatus.Pending || 
                                o.Status == OrderStatus.Confirmed || 
                                o.Status == OrderStatus.Preparing || 
                                o.Status == OrderStatus.Ready || 
                                o.Status == OrderStatus.Served));

            var result = new RestaurantDto
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
                TotalTables = totalTables.Count(),
                AvailableTables = availableTables.Count(),
                TotalMenus = totalMenus.Count(),
                ActiveOrders = activeOrders.Count()
            };

            _logger.LogInformation("Retrieved restaurant: {RestaurantName}", restaurant.Name);
            return result;
        }
    }
}
