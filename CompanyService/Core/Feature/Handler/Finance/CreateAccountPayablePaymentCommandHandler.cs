using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para crear un pago de cuenta por pagar
    /// </summary>
    public class CreateAccountPayablePaymentCommandHandler : IRequestHandler<CreateAccountPayablePaymentCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateAccountPayablePaymentCommandHandler> _logger;

        public CreateAccountPayablePaymentCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateAccountPayablePaymentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateAccountPayablePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating payment for account payable {AccountPayableId} with amount {Amount}", 
                    request.AccountPayableId, request.Amount);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Obtener la cuenta por pagar
                var accountPayable = await _context.AccountsPayables
                    .FirstOrDefaultAsync(ap => ap.Id == request.AccountPayableId && ap.CompanyId == request.CompanyId, cancellationToken);

                if (accountPayable == null)
                {
                    throw new InvalidOperationException($"Account payable with ID {request.AccountPayableId} not found in company {request.CompanyId}.");
                }

                // Validar que el monto sea positivo
                if (request.Amount <= 0)
                {
                    throw new InvalidOperationException("Payment amount must be greater than zero.");
                }

                // Validar que el monto no exceda el saldo pendiente
                var remainingAmount = accountPayable.TotalAmount - accountPayable.PaidAmount;
                if (request.Amount > remainingAmount)
                {
                    throw new InvalidOperationException($"Payment amount {request.Amount} exceeds remaining amount {remainingAmount}.");
                }

                // Validar que la fecha de pago no sea futura
                if (request.PaymentDate > DateTime.UtcNow.Date)
                {
                    throw new InvalidOperationException("Payment date cannot be in the future.");
                }

                // Validar el método de pago
                if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
                {
                    throw new InvalidOperationException($"Invalid payment method: {request.PaymentMethod}");
                }

                // Crear el pago
                var payment = new AccountsPayablePayment
                {
                    Id = Guid.NewGuid(),
                    AccountsPayableId = request.AccountPayableId,
                    Amount = request.Amount,
                    PaymentDate = request.PaymentDate,
                    PaymentMethod = paymentMethod,
                    ReferenceNumber = request.Reference,
                    Notes = string.Empty,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AccountsPayablePayments.Add(payment);

                // Actualizar la cuenta por pagar
                accountPayable.PaidAmount += request.Amount;

                // Actualizar el estado según el saldo
                if (accountPayable.RemainingAmount <= 0)
                {
                    accountPayable.Status = AccountPayableStatus.Paid;
                }
                else if (accountPayable.PaidAmount > 0)
                {
                    accountPayable.Status = AccountPayableStatus.PartiallyPaid;
                }

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created payment {PaymentId} for account payable {AccountPayableId}", 
                    payment.Id, request.AccountPayableId);

                return payment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for account payable {AccountPayableId} in company {CompanyId}", 
                    request.AccountPayableId, request.CompanyId);
                throw;
            }
        }
    }
}
