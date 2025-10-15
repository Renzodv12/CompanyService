using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener una cotización por ID
    /// </summary>
    public class GetQuotationByIdQueryHandler : IRequestHandler<GetQuotationByIdQuery, QuotationResponse?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetQuotationByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuotationResponse?> Handle(GetQuotationByIdQuery request, CancellationToken cancellationToken)
        {
            // Buscar la cotización
            var quotation = await _unitOfWork.Repository<Quotation>()
                .FirstOrDefaultAsync(q => q.Id == request.Id && q.CompanyId == request.CompanyId);

            if (quotation == null)
                return null;

            // Mapear a DTO
            return new QuotationResponse
            {
                Id = quotation.Id,
                CompanyId = quotation.CompanyId,
                CompanyName = quotation.Company?.Name ?? "",
                SupplierId = quotation.SupplierId,
                SupplierName = quotation.Supplier?.Name ?? "",
                QuotationNumber = quotation.QuotationNumber,
                RequestDate = quotation.QuotationDate,
                ResponseDate = quotation.QuotationDate,
                ValidUntil = quotation.ValidUntil,
                Status = quotation.Status,
                SubTotal = quotation.SubTotal,
                TaxAmount = quotation.TaxAmount,
                DiscountAmount = quotation.DiscountAmount,
                TotalAmount = quotation.TotalAmount,
                Notes = quotation.Notes,
                PaymentTerms = quotation.PaymentTerms,
                DeliveryTerms = quotation.DeliveryTerms,
                RequestedByUserId = quotation.RequestedBy,
                RequestedByUserName = quotation.RequestedByUser != null ? $"{quotation.RequestedByUser.FirstName} {quotation.RequestedByUser.LastName}".Trim() : "",
                ReviewedByUserId = quotation.ModifiedBy,
                ReviewedByUserName = quotation.ModifiedByUser != null ? $"{quotation.ModifiedByUser.FirstName} {quotation.ModifiedByUser.LastName}".Trim() : "",
                ReviewedAt = quotation.ModifiedDate,
                PurchaseOrderId = quotation.ConvertedToPurchaseOrderId,
                PurchaseOrderNumber = quotation.ConvertedToPurchaseOrder?.OrderNumber,
                CreatedAt = quotation.CreatedDate,
                UpdatedAt = quotation.ModifiedDate ?? quotation.CreatedDate,
                Items = quotation.Items?.Select(item => new QuotationItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "",
                    ProductSku = item.Product?.SKU ?? "",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.Quantity > 0 && item.UnitPrice > 0 ? (item.DiscountAmount / (item.Quantity * item.UnitPrice)) * 100 : 0,
                    TaxPercentage = item.Quantity > 0 && item.UnitPrice > 0 ? (item.TaxAmount / (item.Quantity * item.UnitPrice)) * 100 : 0,
                    LineTotal = item.LineTotal,
                    Notes = item.Description,
                    Specifications = item.Specifications,
                    LeadTimeDays = item.LeadTimeDays
                }).ToList() ?? new List<QuotationItemResponse>()
            };
        }
    }
}