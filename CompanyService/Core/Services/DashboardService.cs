using CompanyService.Core.Entities;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Report;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid companyId)
        {
            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);
            var thisYear = new DateTime(today.Year, 1, 1);

            // Sales Metrics
            var salesThisMonth = await _unitOfWork.Repository<Sale>()
                .WhereAsync(s => s.CompanyId == companyId && s.SaleDate >= thisMonth);
            
            var salesLastMonth = await _unitOfWork.Repository<Sale>()
                .WhereAsync(s => s.CompanyId == companyId && s.SaleDate >= lastMonth && s.SaleDate < thisMonth);

            var totalSalesThisMonth = salesThisMonth.Sum(s => s.TotalAmount);
            var totalSalesLastMonth = salesLastMonth.Sum(s => s.TotalAmount);
            var salesGrowth = totalSalesLastMonth > 0
                ? ((totalSalesThisMonth - totalSalesLastMonth) / totalSalesLastMonth) * 100
                : 0;

            // Customer Metrics
            var allCustomers = await _unitOfWork.Repository<Customer>()
                .WhereAsync(c => c.CompanyId == companyId);
            
            var newCustomersThisMonth = allCustomers.Count(c => c.CreatedAt >= thisMonth);
            var activeCustomersThisMonth = allCustomers.Count(c => c.IsActive);

            // Product & Inventory Metrics
            var allProducts = await _unitOfWork.Repository<Product>()
                .WhereAsync(p => p.CompanyId == companyId);
            
            var activeProducts = allProducts.Count(p => p.IsActive);
            var lowStockProducts = allProducts.Where(p => p.IsActive && p.Stock <= p.MinStock && p.Stock > 0);
            var outOfStockProducts = allProducts.Where(p => p.IsActive && p.Stock == 0);
            var totalInventoryValue = allProducts.Where(p => p.IsActive).Sum(p => p.Stock * p.Price);

            // Financial Metrics (simplified - would need proper financial entities)
            var totalRevenue = totalSalesThisMonth;
            var totalExpenses = totalRevenue * 0.7m; // Simplified calculation
            var netProfit = totalRevenue - totalExpenses;
            var profitMargin = totalRevenue > 0 ? (netProfit / totalRevenue) * 100 : 0;

            // Budget Metrics
            var budgets = await _unitOfWork.Repository<Budget>()
                .WhereAsync(b => b.CompanyId == companyId);
            
            var activeBudgets = budgets.Count(b => b.Status == 1); // Assuming 1 = Active
            var totalBudgetedAmount = budgets.Sum(b => b.BudgetedAmount);
            var totalActualAmount = budgets.Sum(b => b.ActualAmount);
            var budgetVariance = totalBudgetedAmount - totalActualAmount;

            // CRM Metrics
            var leads = await _unitOfWork.Repository<Lead>()
                .WhereAsync(l => l.CompanyId == companyId);
            
            var newLeadsThisMonth = leads.Count(l => l.CreatedAt >= thisMonth);
            var convertedLeadsThisMonth = leads.Count(l => l.CreatedAt >= thisMonth && l.Status == LeadStatus.Converted);
            var leadConversionRate = newLeadsThisMonth > 0 ? (convertedLeadsThisMonth / (decimal)newLeadsThisMonth) * 100 : 0;

            // Recent Activity
            var recentSales = salesThisMonth
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .Select(s => new RecentSaleDto
                {
                    SaleNumber = s.SaleNumber,
                    CustomerName = allCustomers.FirstOrDefault(c => c.Id == s.CustomerId)?.Name ?? "Unknown",
                    Amount = s.TotalAmount,
                    Date = s.SaleDate
                }).ToList();

            var recentLeads = leads
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .Select(l => new RecentLeadDto
                {
                    Id = l.Id,
                    Name = $"{l.FirstName} {l.LastName}",
                    Email = l.Email,
                    Phone = l.Phone ?? string.Empty,
                    Status = l.Status.ToString(),
                    Source = l.Source.ToString(),
                    CreatedAt = l.CreatedAt,
                    EstimatedValue = l.Score // Using Score as EstimatedValue
                }).ToList();

            var recentBudgets = budgets
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new RecentBudgetDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    Year = b.Year,
                    Month = b.Month ?? 0,
                    BudgetedAmount = b.BudgetedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    VariancePercentage = b.VariancePercentage,
                    Status = GetBudgetStatusText(b.Status),
                    CreatedAt = b.CreatedAt
                }).ToList();

            // Sales Trend (last 6 months)
            var salesTrend = new List<SalesTrendDto>();
            for (int i = 5; i >= 0; i--)
            {
                var periodStart = thisMonth.AddMonths(-i);
                var periodEnd = periodStart.AddMonths(1);
                var periodSales = await _unitOfWork.Repository<Sale>()
                    .WhereAsync(s => s.CompanyId == companyId && s.SaleDate >= periodStart && s.SaleDate < periodEnd);
                
                var previousPeriodStart = periodStart.AddMonths(-1);
                var previousPeriodEnd = periodStart;
                var previousPeriodSales = await _unitOfWork.Repository<Sale>()
                    .WhereAsync(s => s.CompanyId == companyId && s.SaleDate >= previousPeriodStart && s.SaleDate < previousPeriodEnd);
                
                var periodAmount = periodSales.Sum(s => s.TotalAmount);
                var previousAmount = previousPeriodSales.Sum(s => s.TotalAmount);
                var growth = previousAmount > 0 ? ((periodAmount - previousAmount) / previousAmount) * 100 : 0;

                salesTrend.Add(new SalesTrendDto
                {
                    Period = periodStart.ToString("yyyy-MM"),
                    Amount = periodAmount,
                    Count = periodSales.Count(),
                    GrowthPercentage = growth
                });
            }

            // Revenue Trend (last 6 months)
            var revenueTrend = new List<RevenueTrendDto>();
            foreach (var trend in salesTrend)
            {
                var trendExpenses = trend.Amount * 0.7m; // Simplified
                var trendNetProfit = trend.Amount - trendExpenses;
                var trendProfitMargin = trend.Amount > 0 ? (trendNetProfit / trend.Amount) * 100 : 0;

                revenueTrend.Add(new RevenueTrendDto
                {
                    Period = trend.Period,
                    Revenue = trend.Amount,
                    Expenses = trendExpenses,
                    NetProfit = trendNetProfit,
                    ProfitMargin = trendProfitMargin
                });
            }

            // Customer Growth (last 6 months)
            var customerGrowth = new List<CustomerGrowthDto>();
            for (int i = 5; i >= 0; i--)
            {
                var periodStart = thisMonth.AddMonths(-i);
                var periodEnd = periodStart.AddMonths(1);
                var newCustomersInPeriod = allCustomers.Count(c => c.CreatedAt >= periodStart && c.CreatedAt < periodEnd);
                var totalCustomersAtPeriod = allCustomers.Count(c => c.CreatedAt < periodEnd);
                
                var previousPeriodStart = periodStart.AddMonths(-1);
                var totalCustomersPreviousPeriod = allCustomers.Count(c => c.CreatedAt < previousPeriodStart);
                var growth = totalCustomersPreviousPeriod > 0 ? ((totalCustomersAtPeriod - totalCustomersPreviousPeriod) / (decimal)totalCustomersPreviousPeriod) * 100 : 0;

                customerGrowth.Add(new CustomerGrowthDto
                {
                    Period = periodStart.ToString("yyyy-MM"),
                    NewCustomers = newCustomersInPeriod,
                    TotalCustomers = totalCustomersAtPeriod,
                    GrowthPercentage = growth
                });
            }

            // KPIs
            var kpis = new List<KPIDto>
            {
                new KPIDto
                {
                    Name = "Monthly Sales Target",
                    Description = "Achievement of monthly sales target",
                    Value = totalSalesThisMonth,
                    Unit = "$",
                    Target = 100000m, // Example target
                    Achievement = Math.Min(100, (totalSalesThisMonth / 100000m) * 100),
                    Status = totalSalesThisMonth >= 100000m ? "Good" : totalSalesThisMonth >= 80000m ? "Warning" : "Critical",
                    Trend = salesGrowth > 0 ? "Up" : salesGrowth < 0 ? "Down" : "Stable"
                },
                new KPIDto
                {
                    Name = "Customer Growth",
                    Description = "New customers this month",
                    Value = newCustomersThisMonth,
                    Unit = "customers",
                    Target = 20m, // Example target
                    Achievement = Math.Min(100, (newCustomersThisMonth / 20m) * 100),
                    Status = newCustomersThisMonth >= 20 ? "Good" : newCustomersThisMonth >= 15 ? "Warning" : "Critical",
                    Trend = "Up" // Simplified
                },
                new KPIDto
                {
                    Name = "Lead Conversion Rate",
                    Description = "Percentage of leads converted to customers",
                    Value = leadConversionRate,
                    Unit = "%",
                    Target = 15m, // Example target
                    Achievement = Math.Min(100, (leadConversionRate / 15m) * 100),
                    Status = leadConversionRate >= 15 ? "Good" : leadConversionRate >= 10 ? "Warning" : "Critical",
                    Trend = "Stable" // Simplified
                }
            };

            // Alerts
            var alerts = new List<AlertDto>();
            
            if (lowStockProducts.Any())
            {
                alerts.Add(new AlertDto
                {
                    Type = "Warning",
                    Title = "Low Stock Alert",
                    Message = $"{lowStockProducts.Count()} products are running low on stock",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    ActionUrl = "/inventory/products",
                    ActionText = "View Products"
                });
            }

            if (outOfStockProducts.Any())
            {
                alerts.Add(new AlertDto
                {
                    Type = "Critical",
                    Title = "Out of Stock Alert",
                    Message = $"{outOfStockProducts.Count()} products are out of stock",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    ActionUrl = "/inventory/products",
                    ActionText = "Restock Now"
                });
            }

            if (budgetVariance < 0)
            {
                alerts.Add(new AlertDto
                {
                    Type = "Warning",
                    Title = "Budget Variance Alert",
                    Message = $"Actual expenses exceed budget by ${Math.Abs(budgetVariance):N2}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    ActionUrl = "/finance/budgets",
                    ActionText = "Review Budgets"
                });
            }

            return new DashboardSummaryDto
            {
                // Sales Metrics
                TotalSalesThisMonth = totalSalesThisMonth,
                TotalSalesLastMonth = totalSalesLastMonth,
                SalesGrowthPercentage = salesGrowth,
                SalesCountThisMonth = salesThisMonth.Count(),
                SalesCountLastMonth = salesLastMonth.Count(),
                AverageSaleAmount = salesThisMonth.Any() ? salesThisMonth.Average(s => s.TotalAmount) : 0,

                // Customer Metrics
                TotalCustomers = allCustomers.Count(),
                NewCustomersThisMonth = newCustomersThisMonth,
                ActiveCustomersThisMonth = activeCustomersThisMonth,

                // Product & Inventory Metrics
                TotalProducts = allProducts.Count(),
                ActiveProducts = activeProducts,
                LowStockProductsCount = lowStockProducts.Count(),
                OutOfStockProductsCount = outOfStockProducts.Count(),
                TotalInventoryValue = totalInventoryValue,

                // Financial Metrics
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                ProfitMargin = profitMargin,

                // Budget Metrics
                TotalBudgets = budgets.Count(),
                ActiveBudgets = activeBudgets,
                TotalBudgetedAmount = totalBudgetedAmount,
                TotalActualAmount = totalActualAmount,
                BudgetVariance = budgetVariance,

                // CRM Metrics
                TotalLeads = leads.Count(),
                NewLeadsThisMonth = newLeadsThisMonth,
                ConvertedLeadsThisMonth = convertedLeadsThisMonth,
                LeadConversionRate = leadConversionRate,

                // Recent Activity
                RecentSales = recentSales,
                LowStockProducts = lowStockProducts
                    .Take(5)
                    .Select(p => new LowStockProductDto
                    {
                        Name = p.Name,
                        SKU = p.SKU,
                        CurrentStock = p.Stock,
                        MinStock = p.MinStock
                    }).ToList(),
                RecentLeads = recentLeads,
                RecentBudgets = recentBudgets,

                // Charts & Trends
                SalesTrend = salesTrend,
                RevenueTrend = revenueTrend,
                CustomerGrowth = customerGrowth,

                // Performance Indicators
                KPIs = kpis,
                Alerts = alerts
            };
        }

        private string GetBudgetStatusText(int status)
        {
            return status switch
            {
                0 => "Draft",
                1 => "Active",
                2 => "Completed",
                3 => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}
