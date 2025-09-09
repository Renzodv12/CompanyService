using CompanyService.Core.Models.Product;
using FluentValidation;

namespace CompanyService.Core.Validators.Product
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre debe tener máximo 200 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0).WithMessage("El costo debe ser mayor o igual a 0.");

            RuleFor(x => x.MinStock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo debe ser mayor o igual a 0.");

            RuleFor(x => x.MaxStock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock máximo debe ser mayor o igual a 0.")
                .GreaterThanOrEqualTo(x => x.MinStock).WithMessage("El stock máximo debe ser mayor o igual al stock mínimo.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("La categoría es obligatoria.");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("La unidad debe tener máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Unit));

            // Validaciones opcionales para Barcode e ImageUrl
            RuleFor(x => x.Barcode)
                .MaximumLength(50).WithMessage("El código de barras debe tener máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Barcode));

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("La URL de la imagen debe tener máximo 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }
    }
}