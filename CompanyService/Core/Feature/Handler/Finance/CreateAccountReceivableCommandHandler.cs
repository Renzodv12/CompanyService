using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para crear una nueva cuenta por cobrar
    /// </summary>
    public class CreateAccountReceivableCommandHandler : IRequestHandler<CreateAccountReceivableCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateAccountReceivableCommandHandler> _logger;

        public CreateAccountReceivableCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateAccountReceivableCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateAccountReceivableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating account receivable for customer {CustomerId} with invoice {InvoiceNumber}", 
                    request.CustomerId, request.InvoiceNumber);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Validar que el cliente existe
                var customerExists = await _context.Customers
                    .AnyAsync(c => c.Id == request.CustomerId && c.CompanyId == request.CompanyId, cancellationToken);

                if (!customerExists)
                {
                    throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found in company {request.CompanyId}.");
                }

                // Validar que el número de factura no esté duplicado en la compañía
                var invoiceExists = await _context.AccountsReceivables
                    .AnyAsync(ar => ar.CompanyId == request.CompanyId && ar.InvoiceNumber == request.InvoiceNumber, cancellationToken);

                if (invoiceExists)
                {
                    throw new InvalidOperationException($"Invoice number {request.InvoiceNumber} already exists in company {request.CompanyId}.");
                }

                // Validar que el monto sea positivo
                if (request.Amount <= 0)
                {
                    throw new InvalidOperationException("Amount must be greater than zero.");
                }

                // Validar que la fecha de vencimiento sea futura
                if (request.DueDate <= DateTime.UtcNow.Date)
                {
                    throw new InvalidOperationException("Due date must be in the future.");
                }

                // Crear la entidad AccountsReceivable
                var accountReceivable = new AccountsReceivable
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    InvoiceNumber = request.InvoiceNumber,
                    TotalAmount = request.Amount,
                    PaidAmount = 0,
                    DueDate = request.DueDate,
                    Status = AccountReceivableStatus.Pending,
                    Description = request.Description,
                    Notes = string.Empty,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId,
                    IssueDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // Agregar al contexto
                _context.AccountsReceivables.Add(accountReceivable);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created account receivable with ID {AccountReceivableId} for customer {CustomerId}", 
                    accountReceivable.Id, request.CustomerId);

                return accountReceivable.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account receivable for customer {CustomerId} in company {CompanyId}", 
                    request.CustomerId, request.CompanyId);
                throw;
            }
        }
    }
}

