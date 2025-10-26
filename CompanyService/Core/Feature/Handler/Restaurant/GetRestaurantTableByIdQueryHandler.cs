using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantTableByIdQueryHandler : IRequestHandler<GetRestaurantTableByIdQuery, RestaurantTableDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantTableByIdQueryHandler> _logger;

        public GetRestaurantTableByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantTableByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantTableDto?> Handle(GetRestaurantTableByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting table {TableId} for restaurant {RestaurantId}", request.Id, request.RestaurantId);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == request.Id && t.RestaurantId == request.RestaurantId && t.CompanyId == request.CompanyId);

            var table = tables.FirstOrDefault();
            if (table == null)
            {
                return null;
            }

            return new RestaurantTableDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Name = table.Name,
                Capacity = table.Capacity,
                Status = table.Status.ToString(),
                Location = table.Location,
                Description = table.Description,
                IsActive = table.IsActive,
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt,
                RestaurantId = table.RestaurantId,
                RestaurantName = "Restaurant",
                CurrentOrderId = table.CurrentOrderId,
                CurrentOrderNumber = null,
                CurrentGuests = null
            };
        }
    }
}
