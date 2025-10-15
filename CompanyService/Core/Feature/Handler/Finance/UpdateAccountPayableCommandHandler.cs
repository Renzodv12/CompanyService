using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para actualizar una cuenta por pagar existente
    /// </summary>
    public class UpdateAccountPayableCommandHandler : IRequestHandler<UpdateAccountPayableCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateAccountPayableCommandHandler> _logger;

        public UpdateAccountPayableCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateAccountPayableCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateAccountPayableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating account payable {AccountPayableId} for supplier {SupplierId}", 
                    request.Id, request.SupplierId);

                // Obtener la cuenta por pagar existente
                var accountPayable = await _context.AccountsPayables
                    .FirstOrDefaultAsync(ap => ap.Id == request.Id && ap.CompanyId == request.CompanyId, cancellationToken);

                if (accountPayable == null)
                {
                    throw new InvalidOperationException($"Account payable with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Validar que el proveedor existe
                var supplierExists = await _context.Suppliers
                    .AnyAsync(s => s.Id == request.SupplierId && s.CompanyId == request.CompanyId, cancellationToken);

                if (!supplierExists)
                {
                    throw new InvalidOperationException($"Supplier with ID {request.SupplierId} not found in company {request.CompanyId}.");
                }

                // Validar que el número de factura no esté duplicado (excluyendo la cuenta actual)
                var invoiceExists = await _context.AccountsPayables
                    .AnyAsync(ap => ap.CompanyId == request.CompanyId && 
                                   ap.InvoiceNumber == request.InvoiceNumber && 
                                   ap.Id != request.Id, cancellationToken);

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

                // Actualizar la entidad
                accountPayable.SupplierId = request.SupplierId;
                accountPayable.InvoiceNumber = request.InvoiceNumber;
                accountPayable.TotalAmount = request.Amount;
                accountPayable.DueDate = request.DueDate;
                accountPayable.Description = request.Description;
                accountPayable.Notes = request.Notes;
                accountPayable.UpdatedAt = DateTime.UtcNow;

                // Actualizar el estado si es necesario
                if (accountPayable.RemainingAmount <= 0)
                {
                    accountPayable.Status = AccountPayableStatus.Paid;
                }
                else if (accountPayable.PaidAmount > 0)
                {
                    accountPayable.Status = AccountPayableStatus.PartiallyPaid;
                }
                else
                {
                    accountPayable.Status = AccountPayableStatus.Pending;
                }

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated account payable {AccountPayableId} for supplier {SupplierId}", 
                    request.Id, request.SupplierId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account payable {AccountPayableId} in company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}

