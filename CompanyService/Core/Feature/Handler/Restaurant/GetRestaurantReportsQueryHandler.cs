using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;


using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantReportsQueryHandler : IRequestHandler<GetRestaurantReportsQuery, RestaurantReportsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantReportsQueryHandler> _logger;

        public GetRestaurantReportsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantReportsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantReportsDto> Handle(GetRestaurantReportsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting reports for restaurant {RestaurantId}", request.RestaurantId);

            var startDate = request.StartDate ?? DateTime.UtcNow.Date.AddDays(-30);
            var endDate = request.EndDate ?? DateTime.UtcNow.Date;

            // Get orders in date range
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == request.RestaurantId &&
                               o.CreatedAt >= startDate &&
                               o.CreatedAt <= endDate);

            // Get sales in date range
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id != null &&
                               s.CreatedAt >= startDate &&
                               s.CreatedAt <= endDate);

            // Calculate metrics
            var totalOrders = orders.Count();
            var completedOrders = orders.Count(o => o.Status == OrderStatus.Completed);
            var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);
            var totalRevenue = sales.Sum(s => s.TotalAmount);
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Get top selling items
            var saleDetails = await _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>()
                .WhereAsync(sd => sd.SaleId != null);

            var topSellingItems = saleDetails
                .GroupBy(sd => sd.ProductId)
                .Select(g => new TopSellingItemDto
                {
                    ProductId = g.Key,
                    ProductName = "Product", // TODO: Get actual product name
                    TotalQuantity = g.Sum(sd => sd.Quantity),
                    TotalRevenue = g.Sum(sd => sd.Subtotal)
                })
                .OrderByDescending(item => item.TotalQuantity)
                .Take(10)
                .ToList();

            // Get revenue by day
            var revenueByDay = sales
                .GroupBy(s => s.CreatedAt.Date)
                .Select(g => new RevenueByDayDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(s => s.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return new RestaurantReportsDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalOrders = totalOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                TopSellingItems = topSellingItems,
                RevenueByDay = revenueByDay
            };
        }
    }
}
