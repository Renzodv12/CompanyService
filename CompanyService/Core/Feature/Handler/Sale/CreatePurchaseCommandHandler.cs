using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Interfaces;
using MediatR;
using System.Linq;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreatePurchaseCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar proveedor
            var supplier = await _unitOfWork.Repository<CompanyService.Core.Entities.Supplier>()
                .FirstOrDefaultAsync(s => s.Id == request.SupplierId && s.CompanyId == request.CompanyId);

            if (supplier == null)
                throw new DefaultException("Proveedor no encontrado.");

            // Validar productos
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => productIds.Contains(p.Id) && p.CompanyId == request.CompanyId);

            if (products.Count() != request.Items.Count)
                throw new DefaultException("Uno o más productos no fueron encontrados.");

            // Generar número de compra
            var purchaseCount = await _unitOfWork.Repository<CompanyService.Core.Entities.Purchase>()
                .WhereAsync(p => p.CompanyId == request.CompanyId);
            var purchaseNumber = $"C-{DateTime.Now.Year}-{(purchaseCount.Count() + 1):D6}";

            // Calcular totales
            decimal subtotal = 0;
            var purchaseDetails = new List<PurchaseDetail>();

            foreach (var item in request.Items)
            {
                var itemSubtotal = item.UnitCost * item.Quantity;
                subtotal += itemSubtotal;

                purchaseDetails.Add(new PurchaseDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    Subtotal = itemSubtotal
                });
            }

            var taxRate = 0.10m; // IVA 10%
            var taxAmount = subtotal * taxRate;
            var totalAmount = subtotal + taxAmount;

            // Crear compra
            var purchase = new CompanyService.Core.Entities.Purchase
            {
                Id = Guid.NewGuid(),
                PurchaseNumber = purchaseNumber,
                SupplierId = request.SupplierId,
                PurchaseDate = DateTime.UtcNow,
                DeliveryDate = request.DeliveryDate,
                Subtotal = subtotal,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Status = PurchaseStatus.Received, // Asumimos que se recibe inmediatamente
                InvoiceNumber = request.InvoiceNumber,
                Notes = request.Notes,
                CompanyId = request.CompanyId,
                UserId = Guid.Parse(request.UserId),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Purchase>().AddAsync(purchase);

            // Agregar detalles
            foreach (var detail in purchaseDetails)
            {
                detail.PurchaseId = purchase.Id;
                await _unitOfWork.Repository<PurchaseDetail>().AddAsync(detail);
            }

            await _unitOfWork.SaveChangesAsync();

            // Actualizar stock de productos y costos
            foreach (var item in request.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);

                // Actualizar costo del producto
                product.Cost = item.UnitCost;
                product.LastModifiedAt = DateTime.UtcNow;
                _unitOfWork.Repository<CompanyService.Core.Entities.Product>().Update(product);

                // Actualizar stock
                await _mediator.Send(new UpdateStockCommand
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    MovementType = MovementType.Entry,
                    Reason = "Compra",
                    Reference = purchase.PurchaseNumber,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId
                });
            }

            return purchase.Id;
        }
    }
}
