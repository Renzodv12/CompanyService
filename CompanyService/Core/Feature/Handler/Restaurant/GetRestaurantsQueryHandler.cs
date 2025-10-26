using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;
using RestaurantEntity = CompanyService.Core.Entities.Restaurant.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantsQueryHandler : IRequestHandler<GetRestaurantsQuery, List<RestaurantDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantsQueryHandler> _logger;

        public GetRestaurantsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<RestaurantDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting restaurants for company: {CompanyId}", request.CompanyId);

            var restaurants = await _unitOfWork.Repository<RestaurantEntity>()
                .WhereAsync(r => r.CompanyId == request.CompanyId);

            var query = restaurants.AsQueryable();

            if (request.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(r => r.Name.Contains(request.SearchTerm) || 
                                       r.Address.Contains(request.SearchTerm) ||
                                       r.City.Contains(request.SearchTerm));
            }

            var pagedRestaurants = query
                .OrderBy(r => r.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = pagedRestaurants.Select(r => new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Address = r.Address,
                City = r.City,
                Phone = r.Phone,
                Email = r.Email,
                RUC = r.RUC,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                CompanyId = r.CompanyId,
                CreatedBy = r.CreatedBy,
                CreatedByName = "System", // TODO: Get actual user name
                TotalTables = 0, // TODO: Calculate actual values
                AvailableTables = 0,
                TotalMenus = 0,
                ActiveOrders = 0
            }).ToList();

            _logger.LogInformation("Retrieved {Count} restaurants", result.Count);
            return result;
        }
    }
}
