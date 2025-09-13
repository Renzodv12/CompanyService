using FluentValidation;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Procurement
{
    public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
    {
        public CreatePurchaseOrderRequestValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("CompanyId es requerido");

            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("SupplierId es requerido");

            RuleFor(x => x.OrderNumber)
                .NotEmpty()
                .WithMessage("El número de orden es requerido")
                .MaximumLength(50)
                .WithMessage("El número de orden no puede exceder 50 caracteres");

            RuleFor(x => x.OrderDate)
                .NotEmpty()
                .WithMessage("La fecha de orden es requerida");

            RuleFor(x => x.ExpectedDeliveryDate)
                .GreaterThanOrEqualTo(x => x.OrderDate)
                .When(x => x.ExpectedDeliveryDate.HasValue)
                .WithMessage("La fecha esperada de entrega debe ser mayor o igual a la fecha de orden");

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
                .SetValidator(new CreatePurchaseOrderItemRequestValidator());
        }
    }

    public class CreatePurchaseOrderItemRequestValidator : AbstractValidator<CreatePurchaseOrderItemRequest>
    {
        public CreatePurchaseOrderItemRequestValidator()
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

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }

    public class UpdatePurchaseOrderRequestValidator : AbstractValidator<UpdatePurchaseOrderRequest>
    {
        public UpdatePurchaseOrderRequestValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty()
                .WithMessage("SupplierId es requerido");

            RuleFor(x => x.OrderDate)
                .NotEmpty()
                .WithMessage("La fecha de orden es requerida");

            RuleFor(x => x.ExpectedDeliveryDate)
                .GreaterThanOrEqualTo(x => x.OrderDate)
                .When(x => x.ExpectedDeliveryDate.HasValue)
                .WithMessage("La fecha esperada de entrega debe ser mayor o igual a la fecha de orden");

            RuleFor(x => x.PaymentTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de pago no pueden exceder 200 caracteres");

            RuleFor(x => x.DeliveryTerms)
                .MaximumLength(200)
                .WithMessage("Los términos de entrega no pueden exceder 200 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Debe incluir al menos un item")
                .Must(items => items.Count <= 100)
                .WithMessage("No puede incluir más de 100 items");

            RuleForEach(x => x.Items)
                .SetValidator(new UpdatePurchaseOrderItemRequestValidator());
        }
    }

    public class UpdatePurchaseOrderItemRequestValidator : AbstractValidator<UpdatePurchaseOrderItemRequest>
    {
        public UpdatePurchaseOrderItemRequestValidator()
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

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }

    public class UpdatePurchaseOrderStatusRequestValidator : AbstractValidator<UpdatePurchaseOrderStatusRequest>
    {
        public UpdatePurchaseOrderStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado debe ser un valor válido");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");
        }
    }

    public class PurchaseOrderFilterRequestValidator : AbstractValidator<PurchaseOrderFilterRequest>
    {
        public PurchaseOrderFilterRequestValidator()
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

    public class PurchaseOrderReportRequestValidator : AbstractValidator<PurchaseOrderReportRequest>
    {
        public PurchaseOrderReportRequestValidator()
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
}