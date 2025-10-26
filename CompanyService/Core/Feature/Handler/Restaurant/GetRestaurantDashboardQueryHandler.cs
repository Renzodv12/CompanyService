using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantDashboardQueryHandler : IRequestHandler<GetRestaurantDashboardQuery, RestaurantDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantDashboardQueryHandler> _logger;

        public GetRestaurantDashboardQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantDashboardQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantDashboardDto> Handle(GetRestaurantDashboardQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting restaurant dashboard: {RestaurantId}", request.RestaurantId);

            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Table statistics
            var allTables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.RestaurantId == request.RestaurantId);

            var totalTables = allTables.Count();
            var availableTables = allTables.Count(t => t.Status == TableStatus.Available);
            var occupiedTables = allTables.Count(t => t.Status == TableStatus.Occupied);
            var reservedTables = allTables.Count(t => t.Status == TableStatus.Reserved);

            // Order statistics
            var allOrders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == request.RestaurantId);

            var activeOrders = allOrders.Count(o => o.Status == OrderStatus.Pending || 
                                                  o.Status == OrderStatus.Confirmed || 
                                                  o.Status == OrderStatus.Preparing || 
                                                  o.Status == OrderStatus.Ready || 
                                                  o.Status == OrderStatus.Served);

            var pendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending);

            var completedOrdersToday = allOrders.Count(o => o.Status == OrderStatus.Completed && 
                                                           o.CompletedTime.HasValue && 
                                                           o.CompletedTime.Value.Date == today);

            // Recent orders
            var recentOrders = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5);

            var recentOrdersDto = recentOrders.Select(o => new RestaurantOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                Type = o.Type.ToString(),
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                NumberOfGuests = o.NumberOfGuests,
                SubTotal = 0, // Will be calculated from Sale
                TaxAmount = 0, // Will be calculated from Sale
                ServiceCharge = 0, // Will be calculated from Sale
                DiscountAmount = 0, // Will be calculated from Sale
                TotalAmount = 0, // Will be calculated from Sale
                Notes = o.Notes,
                SpecialInstructions = o.SpecialInstructions,
                OrderTime = o.OrderTime,
                EstimatedReadyTime = o.EstimatedReadyTime,
                CompletedTime = o.CompletedTime,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                RestaurantId = o.RestaurantId,
                RestaurantName = "Restaurant", // TODO: Get actual restaurant name
                TableId = o.TableId,
                TableNumber = "Table", // TODO: Get actual table number
                CreatedBy = o.CreatedBy,
                CreatedByName = "System", // TODO: Get actual user name
                AssignedWaiterId = o.AssignedWaiterId,
                AssignedWaiterName = "Waiter", // TODO: Get actual waiter name
                OrderItems = new List<RestaurantOrderItemDto>(),
                Payments = new List<RestaurantPaymentDto>()
            }).ToList();

            // Table status
            var tableStatusDto = allTables.Select(t => new RestaurantTableDto
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
                RestaurantName = "Restaurant", // TODO: Get actual restaurant name
                CurrentOrderId = t.CurrentOrderId,
                CurrentOrderNumber = "Order", // TODO: Get actual order number
                CurrentGuests = 0 // TODO: Get actual guests count
            }).ToList();

            var result = new RestaurantDashboardDto
            {
                TotalTables = totalTables,
                AvailableTables = availableTables,
                OccupiedTables = occupiedTables,
                ReservedTables = reservedTables,
                ActiveOrders = activeOrders,
                PendingOrders = pendingOrders,
                CompletedOrdersToday = completedOrdersToday,
                TotalRevenueToday = 0, // Will be calculated from Sale
                TotalRevenueThisMonth = 0, // Will be calculated from Sale
                RecentOrders = recentOrdersDto,
                TableStatus = tableStatusDto
            };

            _logger.LogInformation("Retrieved restaurant dashboard for: {RestaurantId}", request.RestaurantId);
            return result;
        }
    }
}
