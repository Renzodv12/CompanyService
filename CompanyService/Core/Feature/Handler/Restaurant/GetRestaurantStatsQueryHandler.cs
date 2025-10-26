using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetRestaurantStatsQueryHandler : IRequestHandler<GetRestaurantStatsQuery, RestaurantStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRestaurantStatsQueryHandler> _logger;

        public GetRestaurantStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRestaurantStatsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantStatsDto> Handle(GetRestaurantStatsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting stats for restaurant {RestaurantId}", request.RestaurantId);

            var today = DateTime.UtcNow.Date;
            var thisWeek = today.AddDays(-7);
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Get all orders
            var allOrders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == request.RestaurantId);

            // Get all sales
            var allSales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id != null); // Get all sales for now

            // Calculate today's stats
            var ordersToday = allOrders.Where(o => o.CreatedAt.Date == today);
            var salesToday = allSales.Where(s => s.CreatedAt.Date == today);

            // Calculate this week's stats
            var ordersThisWeek = allOrders.Where(o => o.CreatedAt >= thisWeek);
            var salesThisWeek = allSales.Where(s => s.CreatedAt >= thisWeek);

            // Calculate this month's stats
            var ordersThisMonth = allOrders.Where(o => o.CreatedAt >= thisMonth);
            var salesThisMonth = allSales.Where(s => s.CreatedAt >= thisMonth);

            return new RestaurantStatsDto
            {
                Today = new PeriodStatsDto
                {
                    Orders = ordersToday.Count(),
                    Revenue = salesToday.Sum(s => s.TotalAmount),
                    AverageOrderValue = ordersToday.Any() ? salesToday.Sum(s => s.TotalAmount) / ordersToday.Count() : 0
                },
                ThisWeek = new PeriodStatsDto
                {
                    Orders = ordersThisWeek.Count(),
                    Revenue = salesThisWeek.Sum(s => s.TotalAmount),
                    AverageOrderValue = ordersThisWeek.Any() ? salesThisWeek.Sum(s => s.TotalAmount) / ordersThisWeek.Count() : 0
                },
                ThisMonth = new PeriodStatsDto
                {
                    Orders = ordersThisMonth.Count(),
                    Revenue = salesThisMonth.Sum(s => s.TotalAmount),
                    AverageOrderValue = ordersThisMonth.Any() ? salesThisMonth.Sum(s => s.TotalAmount) / ordersThisMonth.Count() : 0
                },
                AllTime = new PeriodStatsDto
                {
                    Orders = allOrders.Count(),
                    Revenue = allSales.Sum(s => s.TotalAmount),
                    AverageOrderValue = allOrders.Any() ? allSales.Sum(s => s.TotalAmount) / allOrders.Count() : 0
                }
            };
        }
    }
}
