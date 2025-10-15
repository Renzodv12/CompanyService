using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener lista de cotizaciones con filtros
    /// </summary>
    public class GetQuotationsQueryHandler : IRequestHandler<GetQuotationsQuery, List<QuotationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetQuotationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QuotationResponse>> Handle(GetQuotationsQuery request, CancellationToken cancellationToken)
        {
            var quotations = await _unitOfWork.Repository<Quotation>()
                .GetAllAsync();
            
            var filteredQuotations = quotations.Where(q => q.CompanyId == request.CompanyId);

            // Aplicar filtros si existen
            if (request.Filter != null)
            {
                if (request.Filter.SupplierId.HasValue)
                    filteredQuotations = filteredQuotations.Where(q => q.SupplierId == request.Filter.SupplierId.Value);

                if (request.Filter.Status.HasValue)
                    filteredQuotations = filteredQuotations.Where(q => q.Status == request.Filter.Status.Value);

                if (request.Filter.StartDate.HasValue)
                    filteredQuotations = filteredQuotations.Where(q => q.QuotationDate >= request.Filter.StartDate.Value);

                if (request.Filter.EndDate.HasValue)
                    filteredQuotations = filteredQuotations.Where(q => q.QuotationDate <= request.Filter.EndDate.Value);

                if (request.Filter.MinAmount.HasValue)
                    filteredQuotations = filteredQuotations.Where(q => q.TotalAmount >= request.Filter.MinAmount.Value);

                if (request.Filter.MaxAmount.HasValue)
                    filteredQuotations = filteredQuotations.Where(q => q.TotalAmount <= request.Filter.MaxAmount.Value);

                if (!string.IsNullOrEmpty(request.Filter.SearchTerm))
                {
                    var searchTerm = request.Filter.SearchTerm.ToLower();
                    filteredQuotations = filteredQuotations.Where(q => 
                        q.QuotationNumber.ToLower().Contains(searchTerm) ||
                        (q.Supplier?.Name?.ToLower().Contains(searchTerm) ?? false) ||
                        (q.Notes?.ToLower().Contains(searchTerm) ?? false));
                }

                if (request.Filter.IncludeExpired == false)
                    filteredQuotations = filteredQuotations.Where(q => q.ValidUntil == null || q.ValidUntil >= DateTime.Now);
            }

            var quotationsList = filteredQuotations.ToList();

            return quotationsList.Select(quotation => new QuotationResponse
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
                Items = new List<QuotationItemResponse>() // Lista vacía para la vista de lista
            }).ToList();
        }
    }
}