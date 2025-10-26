using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantOrdersQueryHandler : IRequestHandler<GetRestaurantOrdersQuery, List<RestaurantOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantOrdersQueryHandler> _logger;

        public GetRestaurantOrdersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantOrdersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<RestaurantOrderDto>> Handle(GetRestaurantOrdersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting orders for restaurant {RestaurantId}", request.RestaurantId);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == request.RestaurantId);

            var query = orders.AsQueryable();

            if (request.Status.HasValue)
            {
                var statusEnum = (OrderStatus)request.Status.Value;
                query = query.Where(o => o.Status == statusEnum);
            }

            if (request.Type.HasValue)
            {
                var typeEnum = (OrderType)request.Type.Value;
                query = query.Where(o => o.Type == typeEnum);
            }

            if (request.TableId.HasValue)
            {
                query = query.Where(o => o.TableId == request.TableId.Value);
            }

            var result = query
                .OrderByDescending(o => o.CreatedAt)
                .Take(request.PageSize)
                .Select(o => new RestaurantOrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    Type = o.Type.ToString(),
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    NumberOfGuests = o.NumberOfGuests,
                    SubTotal = 0, // Will be calculated from Sale
                    TaxAmount = 0,
                    ServiceCharge = 0,
                    DiscountAmount = 0,
                    TotalAmount = 0,
                    Notes = o.Notes,
                    SpecialInstructions = o.SpecialInstructions,
                    OrderTime = o.OrderTime,
                    EstimatedReadyTime = o.EstimatedReadyTime,
                    CompletedTime = o.CompletedTime,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RestaurantId = o.RestaurantId,
                    RestaurantName = "Restaurant",
                    TableId = o.TableId,
                    TableNumber = "Table",
                    CreatedBy = o.CreatedBy,
                    CreatedByName = "System",
                    AssignedWaiterId = o.AssignedWaiterId,
                    AssignedWaiterName = null,
                    OrderItems = new List<RestaurantOrderItemDto>(),
                    Payments = new List<RestaurantPaymentDto>()
                })
                .ToList();

            return result;
        }
    }
}

