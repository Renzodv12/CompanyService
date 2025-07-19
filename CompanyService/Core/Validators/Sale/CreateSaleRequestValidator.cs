using CompanyService.Core.Models.Sale;
using FluentValidation;

namespace CompanyService.Core.Validators.Sale
{
    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("El cliente es obligatorio.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Debe incluir al menos un producto.")
                .Must(items => items.Count > 0).WithMessage("La venta debe tener al menos un producto.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("El producto es obligatorio.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.");

                item.RuleFor(i => i.Discount)
                    .GreaterThanOrEqualTo(0).WithMessage("El descuento debe ser mayor o igual a 0.");
            });

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("El descuento total debe ser mayor o igual a 0.");
        }
    }
}
