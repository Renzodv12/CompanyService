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

            // Validar stock suficiente
            foreach (var item in request.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                    throw new DefaultException($"Stock insuficiente para el producto {product.Name}. Stock disponible: {product.Stock}");
            }

            // Generar número de venta
            var saleCount = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.CompanyId == request.CompanyId);
            var saleNumber = $"V-{DateTime.Now.Year}-{(saleCount.Count() + 1):D6}";

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

            // Obtener configuración de impuestos (por simplicidad, usamos 10% IVA)
            var taxRate = 0.10m;
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
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().AddAsync(sale);

            // Agregar detalles
            foreach (var detail in saleDetails)
            {
                detail.SaleId = sale.Id;
                await _unitOfWork.Repository<SaleDetail>().AddAsync(detail);
            }

            await _unitOfWork.SaveChangesAsync();

            // Actualizar stock de productos
            foreach (var item in request.Items)
            {
                await _mediator.Send(new UpdateStockCommand
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    MovementType = MovementType.Exit,
                    Reason = "Venta",
                    Reference = sale.SaleNumber,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId
                });
            }

            // TODO: Si GenerateElectronicInvoice es true, integrar con el servicio de facturación electrónica

            return sale.Id;
        }
    }
}
