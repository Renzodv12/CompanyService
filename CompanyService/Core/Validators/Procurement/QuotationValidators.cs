using FluentValidation;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Procurement
{
    public class CreateQuotationRequestValidator : AbstractValidator<CreateQuotationRequest>
    {
        public CreateQuotationRequestValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0)
                .WithMessage("CompanyId debe ser mayor a 0");

            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("SupplierId es requerido");

            RuleFor(x => x.QuotationNumber)
                .NotEmpty()
                .WithMessage("El número de cotización es requerido")
                .MaximumLength(50)
                .WithMessage("El número de cotización no puede exceder 50 caracteres");

            RuleFor(x => x.RequestDate)
                .NotEmpty()
                .WithMessage("La fecha de solicitud es requerida");

            RuleFor(x => x.ValidUntil)
                .GreaterThan(x => x.RequestDate)
                .WithMessage("La fecha de validez debe ser mayor a la fecha de solicitud");

            RuleFor(x => x.DeliveryTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de entrega no pueden exceder 200 caracteres");

            RuleFor(x => x.PaymentTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de pago no pueden exceder 200 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Debe incluir al menos un item")
                .Must(items => items.Count <= 100)
                .WithMessage("No puede incluir más de 100 items");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateQuotationItemRequestValidator());
        }
    }

    public class CreateQuotationItemRequestValidator : AbstractValidator<CreateQuotationItemRequest>
    {
        public CreateQuotationItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId es requerido");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0")
                .LessThanOrEqualTo(999999)
                .WithMessage("La cantidad no puede exceder 999,999");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio unitario debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El precio unitario no puede exceder 999,999,999");

            RuleFor(x => x.DiscountPercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El porcentaje de descuento debe ser mayor o igual a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El porcentaje de descuento no puede exceder 100%");

            RuleFor(x => x.TaxPercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El porcentaje de impuesto debe ser mayor o igual a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El porcentaje de impuesto no puede exceder 100%");

            RuleFor(x => x.LeadTimeDays)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El tiempo de entrega debe ser mayor o igual a 0")
                .LessThanOrEqualTo(365)
                .WithMessage("El tiempo de entrega no puede exceder 365 días");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }

    public class UpdateQuotationRequestValidator : AbstractValidator<UpdateQuotationRequest>
    {
        public UpdateQuotationRequestValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("SupplierId es requerido");

            RuleFor(x => x.RequestDate)
                .NotEmpty()
                .WithMessage("La fecha de solicitud es requerida");

            RuleFor(x => x.ValidUntil)
                .GreaterThan(x => x.RequestDate)
                .WithMessage("La fecha de validez debe ser mayor a la fecha de solicitud");

            RuleFor(x => x.DeliveryTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de entrega no pueden exceder 200 caracteres");

            RuleFor(x => x.PaymentTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de pago no pueden exceder 200 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Debe incluir al menos un item")
                .Must(items => items.Count <= 100)
                .WithMessage("No puede incluir más de 100 items");

            RuleForEach(x => x.Items)
                .SetValidator(new UpdateQuotationItemRequestValidator());
        }
    }

    public class UpdateQuotationItemRequestValidator : AbstractValidator<UpdateQuotationItemRequest>
    {
        public UpdateQuotationItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId es requerido");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0")
                .LessThanOrEqualTo(999999)
                .WithMessage("La cantidad no puede exceder 999,999");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio unitario debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El precio unitario no puede exceder 999,999,999");

            RuleFor(x => x.DiscountPercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El porcentaje de descuento debe ser mayor o igual a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El porcentaje de descuento no puede exceder 100%");

            RuleFor(x => x.TaxPercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El porcentaje de impuesto debe ser mayor o igual a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El porcentaje de impuesto no puede exceder 100%");

            RuleFor(x => x.LeadTimeDays)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El tiempo de entrega debe ser mayor o igual a 0")
                .LessThanOrEqualTo(365)
                .WithMessage("El tiempo de entrega no puede exceder 365 días");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }

    public class UpdateQuotationStatusRequestValidator : AbstractValidator<UpdateQuotationStatusRequest>
    {
        public UpdateQuotationStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado debe ser un valor válido");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");
        }
    }

    public class ConvertQuotationToPurchaseOrderRequestValidator : AbstractValidator<ConvertQuotationToPurchaseOrderRequest>
    {
        public ConvertQuotationToPurchaseOrderRequestValidator()
        {
            RuleFor(x => x.PurchaseOrderNumber)
                .NotEmpty()
                .WithMessage("El número de orden de compra es requerido")
                .MaximumLength(50)
                .WithMessage("El número de orden de compra no puede exceder 50 caracteres");

            RuleFor(x => x.OrderDate)
                .NotEmpty()
                .WithMessage("La fecha de orden es requerida");

            RuleFor(x => x.RequiredDate)
                .GreaterThanOrEqualTo(x => x.OrderDate)
                .WithMessage("La fecha requerida debe ser mayor o igual a la fecha de orden");

            // DeliveryAddress property not available in ConvertQuotationToPurchaseOrderRequest

            RuleFor(x => x.PaymentTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de pago no pueden exceder 200 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.SelectedItems)
                .NotEmpty()
                .WithMessage("Debe seleccionar al menos un item para convertir");

            RuleForEach(x => x.SelectedItems)
                .NotEmpty()
                .WithMessage("Los IDs de items seleccionados no pueden estar vacíos");
        }
    }

    public class QuotationFilterRequestValidator : AbstractValidator<QuotationFilterRequest>
    {
        public QuotationFilterRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("El estado debe ser un valor válido");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("La fecha de inicio debe ser menor o igual a la fecha de fin");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("El término de búsqueda no puede exceder 100 caracteres");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("La página debe ser mayor a 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("El tamaño de página debe ser mayor a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El tamaño de página no puede exceder 100");
        }
    }

    public class QuotationReportRequestValidator : AbstractValidator<QuotationReportRequest>
    {
        public QuotationReportRequestValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("La fecha de inicio es requerida")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("La fecha de inicio debe ser menor o igual a la fecha de fin");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("La fecha de fin es requerida")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("La fecha de fin debe ser mayor o igual a la fecha de inicio");

            RuleFor(x => x.EndDate)
                .Must((request, endDate) => (endDate - request.StartDate).TotalDays <= 365)
                .WithMessage("El rango de fechas no puede exceder 365 días");
        }
    }

    public class QuotationComparisonRequestValidator : AbstractValidator<QuotationComparisonRequest>
    {
        public QuotationComparisonRequestValidator()
        {
            RuleFor(x => x.QuotationIds)
                .NotEmpty()
                .WithMessage("Debe incluir al menos una cotización para comparar")
                .Must(ids => ids.Count >= 2)
                .WithMessage("Debe incluir al menos 2 cotizaciones para comparar")
                .Must(ids => ids.Count <= 10)
                .WithMessage("No puede comparar más de 10 cotizaciones a la vez");

            RuleForEach(x => x.QuotationIds)
                .NotEmpty()
                .WithMessage("Los IDs de cotización no pueden estar vacíos");
        }
    }
}