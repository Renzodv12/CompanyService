using MediatR;
using CompanyService.Core.Feature.Commands.Sale;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Sale
{
    /// <summary>
    /// Handler para eliminar una compra existente
    /// </summary>
    public class DeletePurchaseCommandHandler : IRequestHandler<DeletePurchaseCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeletePurchaseCommandHandler> _logger;

        public DeletePurchaseCommandHandler(
            ApplicationDbContext context,
            ILogger<DeletePurchaseCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting purchase {PurchaseId} from company {CompanyId}", 
                    request.Id, request.CompanyId);

                // Obtener la compra existente con sus items
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseDetails)
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == request.CompanyId, cancellationToken);

                if (purchase == null)
                {
                    throw new InvalidOperationException($"Purchase with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Verificar si la compra puede ser eliminada (por ejemplo, si no está procesada)
                // Aquí puedes agregar validaciones específicas según tus reglas de negocio
                // Por ejemplo: if (purchase.Status == PurchaseStatus.Processed) { throw new InvalidOperationException("Cannot delete processed purchase"); }

                // Eliminar items primero (por las relaciones de clave foránea)
                if (purchase.PurchaseDetails.Any())
                {
                    _context.PurchaseDetails.RemoveRange(purchase.PurchaseDetails);
                }

                // Eliminar la compra
                _context.Purchases.Remove(purchase);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted purchase {PurchaseId} from company {CompanyId}", 
                    request.Id, request.CompanyId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase {PurchaseId} in company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
