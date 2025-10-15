using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener la lista de cuentas por cobrar
    /// </summary>
    public class GetAccountsReceivableQueryHandler : IRequestHandler<GetAccountsReceivableQuery, List<AccountsReceivableResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAccountsReceivableQueryHandler> _logger;

        public GetAccountsReceivableQueryHandler(
            ApplicationDbContext context,
            ILogger<GetAccountsReceivableQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AccountsReceivableResponseDto>> Handle(GetAccountsReceivableQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting accounts receivable for company {CompanyId}", request.CompanyId);

                var accountsReceivable = await _context.AccountsReceivables
                    .Include(ar => ar.Customer)
                    .Include(ar => ar.Payments)
                    .Where(ar => ar.CompanyId == request.CompanyId)
                    .OrderByDescending(ar => ar.DueDate)
                    .ThenBy(ar => ar.InvoiceNumber)
                    .Select(ar => new AccountsReceivableResponseDto
                    {
                        Id = ar.Id,
                        InvoiceNumber = ar.InvoiceNumber,
                        CustomerId = ar.CustomerId,
                        CustomerName = ar.Customer != null ? ar.Customer.Name : "N/A",
                        SaleId = ar.SaleId,
                        TotalAmount = ar.TotalAmount,
                        PaidAmount = ar.PaidAmount,
                        RemainingAmount = ar.RemainingAmount,
                        IssueDate = ar.IssueDate,
                        DueDate = ar.DueDate,
                        Status = ar.Status,
                        StatusName = ar.Status.ToString(),
                        Description = ar.Description,
                        Notes = ar.Notes,
                        IsOverdue = ar.DueDate < DateTime.UtcNow && ar.Status != CompanyService.Core.Enums.AccountReceivableStatus.Paid,
                        DaysOverdue = ar.DueDate < DateTime.UtcNow && ar.Status != CompanyService.Core.Enums.AccountReceivableStatus.Paid 
                            ? (DateTime.UtcNow - ar.DueDate).Days 
                            : 0,
                        CreatedAt = ar.CreatedAt,
                        UpdatedAt = ar.UpdatedAt,
                        Payments = ar.Payments.Select(p => new AccountsReceivablePaymentResponseDto
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

                _logger.LogInformation("Successfully retrieved {Count} accounts receivable for company {CompanyId}", 
                    accountsReceivable.Count, request.CompanyId);

                return accountsReceivable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts receivable for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}

