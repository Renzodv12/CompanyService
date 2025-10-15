using MediatR;
using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Models.Sale;
using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Sale
{
    /// <summary>
    /// Handler para obtener una compra por ID
    /// </summary>
    public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, PurchaseDetailDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetPurchaseByIdQueryHandler> _logger;

        public GetPurchaseByIdQueryHandler(
            ApplicationDbContext context,
            ILogger<GetPurchaseByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PurchaseDetailDto?> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting purchase {PurchaseId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var purchase = await _context.Purchases
                    .Include(p => p.Supplier)
                    .Include(p => p.PurchaseDetails)
                        .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == request.CompanyId, cancellationToken);

                if (purchase == null)
                {
                    _logger.LogWarning("Purchase {PurchaseId} not found in company {CompanyId}", 
                        request.Id, request.CompanyId);
                    return null;
                }

                var purchaseDto = new PurchaseDetailDto
                {
                    Id = purchase.Id,
                    PurchaseNumber = purchase.PurchaseNumber ?? string.Empty,
                    SupplierId = purchase.SupplierId,
                    SupplierName = purchase.Supplier?.Name ?? string.Empty,
                    PurchaseDate = purchase.PurchaseDate,
                    DeliveryDate = purchase.DeliveryDate,
                    InvoiceNumber = purchase.InvoiceNumber,
                    Notes = purchase.Notes,
                    TotalAmount = purchase.PurchaseDetails.Sum(i => i.Quantity * i.UnitCost),
                    Status = purchase.Status,
                    CreatedDate = purchase.CreatedAt,
                    CreatedByUserName = string.Empty, // TODO: Add User navigation property to Purchase entity
                    Items = purchase.PurchaseDetails.Select(i => new PurchaseItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? string.Empty,
                        ProductSKU = i.Product?.SKU ?? string.Empty,
                        Quantity = i.Quantity,
                        UnitCost = i.UnitCost,
                        TotalCost = i.Quantity * i.UnitCost
                    }).ToList()
                };

                _logger.LogInformation("Successfully retrieved purchase {PurchaseId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                return purchaseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase {PurchaseId} in company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
