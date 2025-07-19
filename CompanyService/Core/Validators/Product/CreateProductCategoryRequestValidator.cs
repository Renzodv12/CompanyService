using CompanyService.Core.Models.Product;
using FluentValidation;

namespace CompanyService.Core.Validators.Product
{
    public class CreateProductCategoryRequestValidator : AbstractValidator<CreateProductCategoryRequest>
    {
        public CreateProductCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre debe tener máximo 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("La descripción debe tener máximo 300 caracteres.");
        }
    }
}
