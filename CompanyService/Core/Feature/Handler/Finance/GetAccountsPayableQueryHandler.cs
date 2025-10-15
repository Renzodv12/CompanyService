using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener la lista de cuentas por pagar
    /// </summary>
    public class GetAccountsPayableQueryHandler : IRequestHandler<GetAccountsPayableQuery, List<AccountsPayableResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAccountsPayableQueryHandler> _logger;

        public GetAccountsPayableQueryHandler(
            ApplicationDbContext context,
            ILogger<GetAccountsPayableQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AccountsPayableResponseDto>> Handle(GetAccountsPayableQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting accounts payable for company {CompanyId}", request.CompanyId);

                var accountsPayable = await _context.AccountsPayables
                    .Include(ap => ap.Supplier)
                    .Include(ap => ap.Payments)
                    .Where(ap => ap.CompanyId == request.CompanyId)
                    .OrderByDescending(ap => ap.DueDate)
                    .ThenBy(ap => ap.InvoiceNumber)
                    .Select(ap => new AccountsPayableResponseDto
                    {
                        Id = ap.Id,
                        InvoiceNumber = ap.InvoiceNumber,
                        SupplierId = ap.SupplierId,
                        SupplierName = ap.Supplier != null ? ap.Supplier.Name : "N/A",
                        PurchaseId = ap.PurchaseId,
                        TotalAmount = ap.TotalAmount,
                        PaidAmount = ap.PaidAmount,
                        RemainingAmount = ap.RemainingAmount,
                        IssueDate = ap.IssueDate,
                        DueDate = ap.DueDate,
                        Status = ap.Status,
                        StatusName = ap.Status.ToString(),
                        Description = ap.Description,
                        Notes = ap.Notes,
                        IsOverdue = ap.DueDate < DateTime.UtcNow && ap.Status != CompanyService.Core.Enums.AccountPayableStatus.Paid,
                        DaysOverdue = ap.DueDate < DateTime.UtcNow && ap.Status != CompanyService.Core.Enums.AccountPayableStatus.Paid 
                            ? (DateTime.UtcNow - ap.DueDate).Days 
                            : 0,
                        CreatedAt = ap.CreatedAt,
                        UpdatedAt = ap.UpdatedAt,
                        Payments = ap.Payments.Select(p => new AccountsPayablePaymentResponseDto
                        {
                            Id = p.Id,
                            Amount = p.Amount,
                            PaymentDate = p.PaymentDate,
                            PaymentMethod = p.PaymentMethod,
                            PaymentMethodName = p.PaymentMethod.ToString(),
                            ReferenceNumber = p.ReferenceNumber,
                            Notes = p.Notes,
                            CreatedAt = p.CreatedAt
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} accounts payable for company {CompanyId}", 
                    accountsPayable.Count, request.CompanyId);

                return accountsPayable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts payable for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}

