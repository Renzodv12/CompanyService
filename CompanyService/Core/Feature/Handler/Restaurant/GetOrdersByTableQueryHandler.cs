using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetOrdersByTableQueryHandler : IRequestHandler<GetOrdersByTableQuery, List<RestaurantOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetOrdersByTableQueryHandler> _logger;

        public GetOrdersByTableQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrdersByTableQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<RestaurantOrderDto>> Handle(GetOrdersByTableQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting orders for table {TableId}", request.TableId);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.TableId == request.TableId && o.RestaurantId == request.RestaurantId);

            var result = orders
                .OrderByDescending(o => o.CreatedAt)
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

