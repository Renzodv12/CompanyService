using CompanyService.Core.Models.Sale;
using FluentValidation;

namespace CompanyService.Core.Validators.Sale
{
    public class CreatePurchaseRequestValidator : AbstractValidator<CreatePurchaseRequest>
    {
        public CreatePurchaseRequestValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("El proveedor es obligatorio.");

            RuleFor(x => x.DeliveryDate)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("La fecha de entrega debe ser hoy o posterior.");

            RuleFor(x => x.InvoiceNumber)
                .MaximumLength(50).WithMessage("El número de factura debe tener máximo 50 caracteres.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Debe incluir al menos un producto.")
                .Must(items => items.Count > 0).WithMessage("La compra debe tener al menos un producto.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("El producto es obligatorio.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

                item.RuleFor(i => i.UnitCost)
                    .GreaterThan(0).WithMessage("El costo unitario debe ser mayor a 0.");
            });
        }
    }
}
