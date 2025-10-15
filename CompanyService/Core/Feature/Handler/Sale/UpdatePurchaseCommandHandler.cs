using MediatR;
using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Sale
{
    /// <summary>
    /// Handler para actualizar una compra existente
    /// </summary>
    public class UpdatePurchaseCommandHandler : IRequestHandler<UpdatePurchaseCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdatePurchaseCommandHandler> _logger;

        public UpdatePurchaseCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdatePurchaseCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating purchase {PurchaseId} for supplier {SupplierId}", 
                    request.Id, request.SupplierId);

                // Obtener la compra existente
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseDetails)
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == request.CompanyId, cancellationToken);

                if (purchase == null)
                {
                    throw new InvalidOperationException($"Purchase with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Validar que el proveedor existe
                var supplierExists = await _context.Suppliers
                    .AnyAsync(s => s.Id == request.SupplierId && s.CompanyId == request.CompanyId, cancellationToken);

                if (!supplierExists)
                {
                    throw new InvalidOperationException($"Supplier with ID {request.SupplierId} not found in company {request.CompanyId}.");
                }

                // Validar que el número de factura no esté duplicado (excluyendo la compra actual)
                var invoiceExists = await _context.Purchases
                    .AnyAsync(p => p.CompanyId == request.CompanyId && 
                                  p.InvoiceNumber == request.InvoiceNumber && 
                                  p.Id != request.Id, cancellationToken);

                if (invoiceExists)
                {
                    throw new InvalidOperationException($"Invoice number {request.InvoiceNumber} already exists in company {request.CompanyId}.");
                }

                // Validar que hay items
                if (request.Items == null || !request.Items.Any())
                {
                    throw new InvalidOperationException("Purchase must have at least one item.");
                }

                // Validar que todos los productos existen
                var productIds = request.Items.Select(i => i.ProductId).ToList();
                var existingProducts = await _context.Products
                    .Where(p => productIds.Contains(p.Id) && p.CompanyId == request.CompanyId)
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                var missingProducts = productIds.Except(existingProducts).ToList();
                if (missingProducts.Any())
                {
                    throw new InvalidOperationException($"Products not found: {string.Join(", ", missingProducts)}");
                }

                // Actualizar la entidad principal
                purchase.SupplierId = request.SupplierId;
                purchase.DeliveryDate = request.DeliveryDate;
                purchase.InvoiceNumber = request.InvoiceNumber;
                purchase.Notes = request.Notes;
                // Note: Purchase entity doesn't have UpdatedDate property

                // Eliminar items existentes
                _context.PurchaseDetails.RemoveRange(purchase.PurchaseDetails);

                // Agregar nuevos items
                foreach (var itemRequest in request.Items)
                {
                    var item = new PurchaseDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseId = purchase.Id,
                        ProductId = itemRequest.ProductId,
                        Quantity = itemRequest.Quantity,
                        UnitCost = itemRequest.UnitCost,
                        Subtotal = itemRequest.Quantity * itemRequest.UnitCost
                    };

                    _context.PurchaseDetails.Add(item);
                }

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated purchase {PurchaseId} for supplier {SupplierId}", 
                    request.Id, request.SupplierId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase {PurchaseId} in company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
