using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetAvailableTablesQueryHandler : IRequestHandler<GetAvailableTablesQuery, List<RestaurantTableDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAvailableTablesQueryHandler> _logger;

        public GetAvailableTablesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAvailableTablesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<RestaurantTableDto>> Handle(GetAvailableTablesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting available tables for restaurant {RestaurantId}", request.RestaurantId);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.RestaurantId == request.RestaurantId &&
                                 t.CompanyId == request.CompanyId &&
                                 t.Status == TableStatus.Available &&
                                 (!request.MinCapacity.HasValue || t.Capacity >= request.MinCapacity.Value));

            return tables
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
                    RestaurantName = "Restaurant",
                    CurrentOrderId = t.CurrentOrderId,
                    CurrentOrderNumber = null,
                    CurrentGuests = null
                })
                .ToList();
        }
    }
}
