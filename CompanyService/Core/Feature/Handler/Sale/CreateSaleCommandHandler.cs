using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateSaleCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar cliente
            var customer = await _unitOfWork.Repository<CompanyService.Core.Entities.Customer>()
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.CompanyId == request.CompanyId);

            if (customer == null)
                throw new DefaultException("Cliente no encontrado.");

            // Validar productos y stock
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => productIds.Contains(p.Id) && p.CompanyId == request.CompanyId);

            if (products.Count() != request.Items.Count)
                throw new DefaultException("Uno o más productos no fueron encontrados.");

            // Validar stock suficiente y reglas de negocio
            foreach (var item in request.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                
                // Validar stock suficiente
                if (product.Stock < item.Quantity)
                    throw new DefaultException($"Stock insuficiente para el producto {product.Name}. Stock disponible: {product.Stock}");
                
                // Validar que el precio unitario no sea negativo
                if (item.UnitPrice < 0)
                    throw new DefaultException($"El precio unitario del producto {product.Name} no puede ser negativo");
                
                // Validar que el descuento no sea mayor al subtotal del item
                var itemSubtotal = item.UnitPrice * item.Quantity;
                if (item.Discount > itemSubtotal)
                    throw new DefaultException($"El descuento del producto {product.Name} no puede ser mayor al subtotal del item");
                
                // Validar que la cantidad sea positiva
                if (item.Quantity <= 0)
                    throw new DefaultException($"La cantidad del producto {product.Name} debe ser mayor a cero");
            }
            
            // Validar que el descuento total no sea mayor al subtotal
            var totalSubtotal = request.Items.Sum(i => (i.UnitPrice * i.Quantity) - i.Discount);
            if (request.DiscountAmount > totalSubtotal)
                throw new DefaultException("El descuento total no puede ser mayor al subtotal");

            // Generar número de venta de manera eficiente
            //var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
            //    .WhereAsync(s => s.CompanyId == request.CompanyId && s.SaleNumber.StartsWith($"V-{DateTime.Now.Year}-"));
            var prefix = $"V-{DateTime.Now.Year}-";

            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.CompanyId == request.CompanyId && s.SaleNumber.Contains(prefix));


            var lastSaleNumber = sales.OrderByDescending(s => s.SaleNumber).FirstOrDefault();
            
            int nextNumber = 1;
            if (lastSaleNumber != null)
            {
                var lastNumberStr = lastSaleNumber.SaleNumber.Split('-').LastOrDefault();
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            
            var saleNumber = $"V-{DateTime.Now.Year}-{nextNumber:D6}";

            // Calcular totales
            decimal subtotal = 0;
            var saleDetails = new List<SaleDetail>();

            foreach (var item in request.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                var itemSubtotal = (item.UnitPrice * item.Quantity) - item.Discount;
                subtotal += itemSubtotal;

                saleDetails.Add(new SaleDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    Subtotal = itemSubtotal
                });
            }

            // Obtener configuración de impuestos de la empresa
            var companySettings = await _unitOfWork.Repository<CompanySettings>()
                .FirstOrDefaultAsync(cs => cs.CompanyId == request.CompanyId);
            
            var taxRate = 0.10m; // Default 10% - TODO: Implementar configuración de impuestos por empresa
            var taxAmount = subtotal * taxRate;
            var totalAmount = subtotal + taxAmount - request.DiscountAmount;

            // Crear venta
            var sale = new CompanyService.Core.Entities.Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = saleNumber,
                CustomerId = request.CustomerId,
                SaleDate = DateTime.UtcNow,
                Subtotal = subtotal,
                TaxAmount = taxAmount,
                DiscountAmount = request.DiscountAmount,
                TotalAmount = totalAmount,
                Status = SaleStatus.Completed,
                PaymentMethod = request.PaymentMethod,
                Notes = request.Notes,
                CompanyId = request.CompanyId,
                UserId = Guid.Parse(request.UserId),
                IsElectronicInvoice = request.GenerateElectronicInvoice,
                ElectronicInvoiceId = request.GenerateElectronicInvoice ? Guid.NewGuid().ToString() : null,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().AddAsync(sale);

            // Agregar detalles
            foreach (var detail in saleDetails)
            {
                detail.SaleId = sale.Id;
                await _unitOfWork.Repository<SaleDetail>().AddAsync(detail);
            }

            // Actualizar stock de productos ANTES de guardar la venta
            foreach (var item in request.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                var previousStock = product.Stock;
                var newStock = previousStock - item.Quantity;

                // Actualizar stock del producto
                product.Stock = newStock;
                product.LastModifiedAt = DateTime.UtcNow;
                _unitOfWork.Repository<CompanyService.Core.Entities.Product>().Update(product);

                // Crear movimiento de stock
                var stockMovement = new StockMovement
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Type = MovementType.Exit,
                    Quantity = item.Quantity,
                    PreviousStock = previousStock,
                    NewStock = newStock,
                    Reason = "Venta",
                    Reference = sale.SaleNumber,
                    UserId = Guid.Parse(request.UserId),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<StockMovement>().AddAsync(stockMovement);
            }

            // Guardar todo en una sola transacción
            await _unitOfWork.SaveChangesAsync();

            // TODO: Si GenerateElectronicInvoice es true, integrar con el servicio de facturación electrónica

            return sale.Id;
        }
    }
}
