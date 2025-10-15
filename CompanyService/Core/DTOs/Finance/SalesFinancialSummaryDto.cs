namespace CompanyService.Core.DTOs.Finance
{
    /// <summary>
    /// DTO para el resumen financiero de ventas
    /// </summary>
    public class SalesFinancialSummaryDto
    {
        public PeriodDto Period { get; set; } = new();
        public SalesMetricsDto SalesMetrics { get; set; } = new();
        public CollectionMetricsDto CollectionMetrics { get; set; } = new();
        public List<PaymentMethodSummaryDto> SalesByPaymentMethod { get; set; } = new();
        public List<ReceivableStatusSummaryDto> ReceivableStatus { get; set; } = new();
        public List<SalePendingPaymentDto> SalesPendingPayment { get; set; } = new();
    }

    /// <summary>
    /// DTO para el período de análisis
    /// </summary>
    public class PeriodDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    /// <summary>
    /// DTO para métricas de ventas
    /// </summary>
    public class SalesMetricsDto
    {
        public decimal TotalSales { get; set; }
        public int TotalSalesCount { get; set; }
        public int CompletedSales { get; set; }
        public int PendingSales { get; set; }
        public int CancelledSales { get; set; }
        public decimal AverageSaleAmount { get; set; }
    }

    /// <summary>
    /// DTO para métricas de cobranza
    /// </summary>
    public class CollectionMetricsDto
    {
        public decimal TotalReceivable { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }
        public decimal CollectionRate { get; set; }
        public decimal OverdueAmount { get; set; }
        public int OverdueCount { get; set; }
    }

    /// <summary>
    /// DTO para resumen por método de pago
    /// </summary>
    public class PaymentMethodSummaryDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// DTO para resumen de estado de cuentas por cobrar
    /// </summary>
    public class ReceivableStatusSummaryDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    /// <summary>
    /// DTO para ventas pendientes de pago
    /// </summary>
    public class SalePendingPaymentDto
    {
        public Guid SaleId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DaysOverdue { get; set; }
    }
}
