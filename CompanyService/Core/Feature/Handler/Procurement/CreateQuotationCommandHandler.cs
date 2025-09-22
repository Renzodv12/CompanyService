using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para crear una nueva cotización
    /// </summary>
    public class CreateQuotationCommandHandler : IRequestHandler<CreateQuotationCommand, QuotationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuotationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuotationResponse> Handle(CreateQuotationCommand request, CancellationToken cancellationToken)
        {
            // Validar que la empresa existe
            var company = await _unitOfWork.Repository<Entities.Company>()
                .FirstOrDefaultAsync(c => c.Id == request.CompanyId);

            if (company == null)
                throw new DefaultException("Empresa no encontrada.");

            // Validar que el proveedor existe
            var supplier = await _unitOfWork.Repository<Supplier>()
                .FirstOrDefaultAsync(s => s.Id == request.Request.SupplierId);

            if (supplier == null)
                throw new DefaultException("Proveedor no encontrado.");

            // Crear la cotización
            var quotation = new Quotation
            {
                Id = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                SupplierId = request.Request.SupplierId,
                QuotationNumber = request.Request.QuotationNumber,
                QuotationDate = request.Request.RequestDate,
                ValidUntil = request.Request.ValidUntil,
                Status = QuotationStatus.Draft,
                SubTotal = request.Request.SubTotal,
                TaxAmount = request.Request.TaxAmount,
                DiscountAmount = request.Request.DiscountAmount,
                TotalAmount = request.Request.TotalAmount,
                Notes = request.Request.Notes,
                PaymentTerms = request.Request.PaymentTerms,
                DeliveryTerms = request.Request.DeliveryTerms,
                CreatedDate = DateTime.UtcNow,
                Items = request.Request.Items.Select(item => new QuotationItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.Quantity * item.UnitPrice * (item.DiscountPercentage / 100),
                    TaxAmount = item.Quantity * item.UnitPrice * (item.TaxPercentage / 100),
                    LineTotal = item.LineTotal,
                    Description = item.Notes,
                    Specifications = item.Specifications,
                    LeadTimeDays = item.LeadTimeDays
                }).ToList()
            };

            await _unitOfWork.Repository<Quotation>().AddAsync(quotation);
            await _unitOfWork.SaveChangesAsync();

            // Retornar la respuesta
            return new QuotationResponse
            {
                Id = quotation.Id,
                CompanyId = quotation.CompanyId,
                CompanyName = company.Name,
                SupplierId = quotation.SupplierId,
                SupplierName = supplier.Name,
                QuotationNumber = quotation.QuotationNumber,
                RequestDate = quotation.QuotationDate,
                ValidUntil = quotation.ValidUntil,
                Status = quotation.Status,
                SubTotal = quotation.SubTotal,
                TaxAmount = quotation.TaxAmount,
                DiscountAmount = quotation.DiscountAmount,
                TotalAmount = quotation.TotalAmount,
                Notes = quotation.Notes,
                PaymentTerms = quotation.PaymentTerms,
                DeliveryTerms = quotation.DeliveryTerms,
                CreatedAt = quotation.CreatedDate,
                UpdatedAt = quotation.ModifiedDate ?? quotation.CreatedDate,
                Items = quotation.Items.Select(item => new QuotationItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.Quantity > 0 && item.UnitPrice > 0 ? (item.DiscountAmount / (item.Quantity * item.UnitPrice)) * 100 : 0,
                    TaxPercentage = item.Quantity > 0 && item.UnitPrice > 0 ? (item.TaxAmount / (item.Quantity * item.UnitPrice)) * 100 : 0,
                    LineTotal = item.LineTotal,
                    Notes = item.Description,
                    Specifications = item.Specifications,
                    LeadTimeDays = item.LeadTimeDays
                }).ToList()
            };
        }
    }
}