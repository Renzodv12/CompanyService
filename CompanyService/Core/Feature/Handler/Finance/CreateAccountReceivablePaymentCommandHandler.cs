using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para crear un nuevo pago de cuenta por cobrar
    /// </summary>
    public class CreateAccountReceivablePaymentCommandHandler : IRequestHandler<CreateAccountReceivablePaymentCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateAccountReceivablePaymentCommandHandler> _logger;

        public CreateAccountReceivablePaymentCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateAccountReceivablePaymentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateAccountReceivablePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating payment for account receivable {AccountReceivableId} with amount {Amount}", 
                    request.AccountReceivableId, request.Amount);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Obtener la cuenta por cobrar con sus pagos
                var accountReceivable = await _context.AccountsReceivables
                    .Include(ar => ar.Payments)
                    .FirstOrDefaultAsync(ar => ar.Id == request.AccountReceivableId && ar.CompanyId == request.CompanyId, cancellationToken);

                if (accountReceivable == null)
                {
                    throw new InvalidOperationException($"Account receivable with ID {request.AccountReceivableId} not found in company {request.CompanyId}.");
                }

                // Validar que el monto sea positivo
                if (request.Amount <= 0)
                {
                    throw new InvalidOperationException("Payment amount must be greater than zero.");
                }

                // Validar que el monto no exceda el monto pendiente
                var remainingAmount = accountReceivable.RemainingAmount;
                if (request.Amount > remainingAmount)
                {
                    throw new InvalidOperationException($"Payment amount {request.Amount} cannot exceed remaining amount {remainingAmount}.");
                }

                // Validar que la fecha de pago no sea futura
                if (request.PaymentDate > DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Payment date cannot be in the future.");
                }

                // Validar el método de pago
                if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
                {
                    throw new InvalidOperationException($"Invalid payment method: {request.PaymentMethod}. Valid methods are: Cash, Card, Transfer, Check, Credit.");
                }

                // Crear el pago
                var payment = new AccountsReceivablePayment
                {
                    Id = Guid.NewGuid(),
                    AccountsReceivableId = request.AccountReceivableId,
                    Amount = request.Amount,
                    PaymentDate = request.PaymentDate,
                    PaymentMethod = paymentMethod,
                    ReferenceNumber = request.Reference,
                    Notes = string.Empty,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                // Agregar el pago al contexto
                _context.AccountsReceivablePayments.Add(payment);

                // Actualizar el monto pagado en la cuenta por cobrar
                accountReceivable.PaidAmount += request.Amount;
                accountReceivable.UpdatedAt = DateTime.UtcNow;

                // Actualizar el estado de la cuenta por cobrar
                if (accountReceivable.RemainingAmount <= 0)
                {
                    accountReceivable.Status = AccountReceivableStatus.Paid;
                }
                else if (accountReceivable.PaidAmount > 0)
                {
                    accountReceivable.Status = AccountReceivableStatus.PartiallyPaid;
                }

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created payment with ID {PaymentId} for account receivable {AccountReceivableId}. " +
                    "New status: {Status}, Paid amount: {PaidAmount}, Remaining: {RemainingAmount}", 
                    payment.Id, request.AccountReceivableId, accountReceivable.Status, 
                    accountReceivable.PaidAmount, accountReceivable.RemainingAmount);

                return payment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for account receivable {AccountReceivableId} in company {CompanyId}", 
                    request.AccountReceivableId, request.CompanyId);
                throw;
            }
        }
    }
}
