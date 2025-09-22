using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para actualizar una cotización existente
    /// </summary>
    public class UpdateQuotationCommandHandler : IRequestHandler<UpdateQuotationCommand, QuotationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuotationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuotationResponse> Handle(UpdateQuotationCommand request, CancellationToken cancellationToken)
        {
            // Buscar la cotización existente
            var quotation = await _unitOfWork.Repository<Quotation>()
                .GetByIdAsync(request.Id);

            if (quotation == null || quotation.CompanyId != request.CompanyId)
                throw new DefaultException("Cotización no encontrada.");

            // Validar que el proveedor existe
            var supplier = await _unitOfWork.Repository<Supplier>()
                .FirstOrDefaultAsync(s => s.Id == request.Request.SupplierId);

            if (supplier == null)
                throw new DefaultException("Proveedor no encontrado.");

            // Actualizar propiedades de la cotización
            quotation.SupplierId = request.Request.SupplierId;
            quotation.QuotationDate = request.Request.RequestDate;
            quotation.ValidUntil = request.Request.ValidUntil;
            quotation.SubTotal = request.Request.SubTotal;
            quotation.TaxAmount = request.Request.TaxAmount;
            quotation.DiscountAmount = request.Request.DiscountAmount;
            quotation.TotalAmount = request.Request.TotalAmount;
            quotation.Notes = request.Request.Notes;
            quotation.PaymentTerms = request.Request.PaymentTerms;
            quotation.DeliveryTerms = request.Request.DeliveryTerms;
            quotation.ModifiedDate = DateTime.UtcNow;

            // Actualizar items - eliminar existentes y agregar nuevos
            quotation.Items.Clear();
            quotation.Items = request.Request.Items.Select(item => new QuotationItem
            {
                Id = item.Id ?? Guid.NewGuid(),
                QuotationId = quotation.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountAmount = item.Quantity * item.UnitPrice * (item.DiscountPercentage / 100),
                TaxAmount = item.Quantity * item.UnitPrice * (item.TaxPercentage / 100),
                LineTotal = item.LineTotal,
                Description = item.Notes,
                Specifications = item.Specifications,
                LeadTimeDays = item.LeadTimeDays
            }).ToList();

            _unitOfWork.Repository<Quotation>().Update(quotation);
            await _unitOfWork.SaveChangesAsync();

            // Retornar la respuesta
            return new QuotationResponse
            {
                Id = quotation.Id,
                CompanyId = quotation.CompanyId,
                CompanyName = quotation.Company?.Name ?? "",
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