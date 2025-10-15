using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener el resumen financiero de ventas
    /// </summary>
    public class GetSalesFinancialSummaryQueryHandler : IRequestHandler<GetSalesFinancialSummaryQuery, SalesFinancialSummaryDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetSalesFinancialSummaryQueryHandler> _logger;

        public GetSalesFinancialSummaryQueryHandler(
            ApplicationDbContext context,
            ILogger<GetSalesFinancialSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SalesFinancialSummaryDto> Handle(GetSalesFinancialSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting sales financial summary for company {CompanyId} from {FromDate} to {ToDate}", 
                    request.CompanyId, request.FromDate, request.ToDate);

                var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
                var toDate = request.ToDate ?? DateTime.UtcNow;

                // Obtener todas las ventas del período
                var sales = await _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.SaleDetails)
                    .Where(s => s.CompanyId == request.CompanyId && 
                               s.SaleDate >= fromDate && 
                               s.SaleDate <= toDate)
                    .ToListAsync(cancellationToken);

                // Obtener cuentas por cobrar relacionadas
                var accountsReceivable = await _context.AccountsReceivables
                    .Include(ar => ar.Sale)
                    .Include(ar => ar.Payments)
                    .Where(ar => ar.CompanyId == request.CompanyId && 
                                ar.SaleId != null &&
                                ar.IssueDate >= fromDate && 
                                ar.IssueDate <= toDate)
                    .ToListAsync(cancellationToken);

                // Calcular métricas
                var totalSales = sales.Sum(s => s.TotalAmount);
                var totalSalesCount = sales.Count;
                var completedSales = sales.Count(s => s.Status == SaleStatus.Completed);
                var pendingSales = sales.Count(s => s.Status == SaleStatus.Pending);
                var cancelledSales = sales.Count(s => s.Status == SaleStatus.Cancelled);

                // Métricas de cobranza
                var totalReceivable = accountsReceivable.Sum(ar => ar.TotalAmount);
                var totalPaid = accountsReceivable.Sum(ar => ar.PaidAmount);
                var totalPending = accountsReceivable.Sum(ar => ar.RemainingAmount);
                var collectionRate = totalReceivable > 0 ? (totalPaid / totalReceivable) * 100 : 0;

                // Ventas por método de pago
                var salesByPaymentMethod = sales
                    .GroupBy(s => s.PaymentMethod)
                    .Select(g => new PaymentMethodSummaryDto
                    {
                        PaymentMethod = g.Key.ToString(),
                        Count = g.Count(),
                        TotalAmount = g.Sum(s => s.TotalAmount),
                        Percentage = totalSales > 0 ? (g.Sum(s => s.TotalAmount) / totalSales) * 100 : 0
                    })
                    .ToList();

                // Estado de cuentas por cobrar
                var receivableStatus = accountsReceivable
                    .GroupBy(ar => ar.Status)
                    .Select(g => new ReceivableStatusSummaryDto
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count(),
                        TotalAmount = g.Sum(ar => ar.TotalAmount),
                        PaidAmount = g.Sum(ar => ar.PaidAmount),
                        RemainingAmount = g.Sum(ar => ar.RemainingAmount)
                    })
                    .ToList();

                // Ventas pendientes de cobro
                var salesPendingPayment = accountsReceivable
                    .Where(ar => ar.RemainingAmount > 0)
                    .Select(ar => new SalePendingPaymentDto
                    {
                        SaleId = ar.SaleId ?? Guid.Empty,
                        SaleNumber = ar.Sale?.SaleNumber ?? string.Empty,
                        CustomerName = ar.Sale?.Customer?.Name ?? string.Empty,
                        TotalAmount = ar.TotalAmount,
                        PaidAmount = ar.PaidAmount,
                        RemainingAmount = ar.RemainingAmount,
                        DueDate = ar.DueDate,
                        Status = ar.Status.ToString(),
                        DaysOverdue = ar.DueDate < DateTime.UtcNow ? (DateTime.UtcNow - ar.DueDate).Days : 0
                    })
                    .ToList();

                var summary = new SalesFinancialSummaryDto
                {
                    Period = new PeriodDto
                    {
                        FromDate = fromDate,
                        ToDate = toDate
                    },
                    SalesMetrics = new SalesMetricsDto
                    {
                        TotalSales = totalSales,
                        TotalSalesCount = totalSalesCount,
                        CompletedSales = completedSales,
                        PendingSales = pendingSales,
                        CancelledSales = cancelledSales,
                        AverageSaleAmount = totalSalesCount > 0 ? totalSales / totalSalesCount : 0
                    },
                    CollectionMetrics = new CollectionMetricsDto
                    {
                        TotalReceivable = totalReceivable,
                        TotalPaid = totalPaid,
                        TotalPending = totalPending,
                        CollectionRate = collectionRate,
                        OverdueAmount = salesPendingPayment.Sum(s => s.RemainingAmount),
                        OverdueCount = salesPendingPayment.Count(s => s.DaysOverdue > 0)
                    },
                    SalesByPaymentMethod = salesByPaymentMethod,
                    ReceivableStatus = receivableStatus,
                    SalesPendingPayment = salesPendingPayment
                };

                _logger.LogInformation("Successfully retrieved sales financial summary for company {CompanyId}. " +
                    "Total sales: {TotalSales}, Collection rate: {CollectionRate}%", 
                    request.CompanyId, totalSales, collectionRate);

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales financial summary for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}
