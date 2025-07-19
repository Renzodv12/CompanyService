using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Sale;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Customer;
using CompanyService.Core.Models.Sale;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDetailDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSaleByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SaleDetailDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            var sale = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == request.CompanyId);

            if (sale == null)
                return null;

            var customer = await _unitOfWork.Repository<Entities.Customer>()
                .FirstOrDefaultAsync(c => c.Id == sale.CustomerId);

            var saleDetails = await _unitOfWork.Repository<SaleDetail>()
                .WhereAsync(sd => sd.SaleId == sale.Id);

            var productIds = saleDetails.Select(sd => sd.ProductId).ToList();
            var products = await _unitOfWork.Repository<CompanyService.Core.Entities.Product>()
                .WhereAsync(p => productIds.Contains(p.Id));

            var items = saleDetails.Select(sd => new SaleItemDto
            {
                ProductName = products.FirstOrDefault(p => p.Id == sd.ProductId)?.Name ?? "",
                ProductSKU = products.FirstOrDefault(p => p.Id == sd.ProductId)?.SKU ?? "",
                Quantity = sd.Quantity,
                UnitPrice = sd.UnitPrice,
                Discount = sd.Discount,
                Subtotal = sd.Subtotal
            }).ToList();

            return new SaleDetailDto
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                Customer = new CustomerDto
                {
                    Id = customer?.Id ?? Guid.Empty,
                    Name = customer?.Name ?? "",
                    DocumentNumber = customer?.DocumentNumber ?? "",
                    DocumentType = customer?.DocumentType ?? Core.Enums.DocumentType.CI,
                    Email = customer?.Email ?? "",
                    Phone = customer?.Phone ?? ""
                },
                SaleDate = sale.SaleDate,
                Subtotal = sale.Subtotal,
                TaxAmount = sale.TaxAmount,
                DiscountAmount = sale.DiscountAmount,
                TotalAmount = sale.TotalAmount,
                Status = sale.Status,
                PaymentMethod = sale.PaymentMethod,
                Notes = sale.Notes,
                IsElectronicInvoice = sale.IsElectronicInvoice,
                ElectronicInvoiceId = sale.ElectronicInvoiceId,
                Items = items
            };
        }
    }
}
