using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para crear una nueva cuenta por pagar
    /// </summary>
    public class CreateAccountPayableCommandHandler : IRequestHandler<CreateAccountPayableCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateAccountPayableCommandHandler> _logger;

        public CreateAccountPayableCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateAccountPayableCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateAccountPayableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating account payable for supplier {SupplierId} with invoice {InvoiceNumber}", 
                    request.SupplierId, request.InvoiceNumber);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Validar que el proveedor existe
                var supplierExists = await _context.Suppliers
                    .AnyAsync(s => s.Id == request.SupplierId && s.CompanyId == request.CompanyId, cancellationToken);

                if (!supplierExists)
                {
                    throw new InvalidOperationException($"Supplier with ID {request.SupplierId} not found in company {request.CompanyId}.");
                }

                // Validar que el número de factura no esté duplicado en la compañía
                var invoiceExists = await _context.AccountsPayables
                    .AnyAsync(ap => ap.CompanyId == request.CompanyId && ap.InvoiceNumber == request.InvoiceNumber, cancellationToken);

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

                // Crear la entidad AccountsPayable
                var accountPayable = new AccountsPayable
                {
                    Id = Guid.NewGuid(),
                    SupplierId = request.SupplierId,
                    InvoiceNumber = request.InvoiceNumber,
                    TotalAmount = request.Amount,
                    PaidAmount = 0,
                    DueDate = request.DueDate,
                    Status = AccountPayableStatus.Pending,
                    Description = request.Description,
                    Notes = string.Empty,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId,
                    IssueDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // Agregar al contexto
                _context.AccountsPayables.Add(accountPayable);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created account payable with ID {AccountPayableId} for supplier {SupplierId}", 
                    accountPayable.Id, request.SupplierId);

                return accountPayable.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account payable for supplier {SupplierId} in company {CompanyId}", 
                    request.SupplierId, request.CompanyId);
                throw;
            }
        }
    }
}
