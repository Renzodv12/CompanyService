using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantTablesQueryHandler : IRequestHandler<GetRestaurantTablesQuery, List<RestaurantTableDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantTablesQueryHandler> _logger;

        public GetRestaurantTablesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantTablesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<RestaurantTableDto>> Handle(GetRestaurantTablesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting tables for restaurant {RestaurantId}", request.RestaurantId);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.RestaurantId == request.RestaurantId && t.CompanyId == request.CompanyId);

            var query = tables.AsQueryable();

            if (request.Status.HasValue)
            {
                var statusEnum = (TableStatus)request.Status.Value;
                query = query.Where(t => t.Status == statusEnum);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var result = query
                .OrderBy(t => t.TableNumber)
                .Select(t => new RestaurantTableDto
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Name = t.Name,
                    Capacity = t.Capacity,
                    Status = t.Status.ToString(),
                    Location = t.Location,
                    Description = t.Description,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RestaurantId = t.RestaurantId,
                    RestaurantName = "Restaurant", // could be enriched if needed
                    CurrentOrderId = t.CurrentOrderId,
                    CurrentOrderNumber = null,
                    CurrentGuests = null
                })
                .ToList();

            return result;
        }
    }
}
