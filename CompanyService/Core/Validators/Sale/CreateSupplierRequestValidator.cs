using CompanyService.Core.Models.Sale;
using FluentValidation;

namespace CompanyService.Core.Validators.Sale
{
    public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
    {
        public CreateSupplierRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del proveedor es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre debe tener máximo 200 caracteres.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(20).WithMessage("El documento debe tener máximo 20 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El email debe tener un formato válido.")
                .MaximumLength(100).WithMessage("El email debe tener máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("El teléfono debe tener máximo 20 caracteres.");

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("La dirección debe tener máximo 300 caracteres.");

            RuleFor(x => x.ContactPerson)
                .MaximumLength(100).WithMessage("La persona de contacto debe tener máximo 100 caracteres.");
        }
    }
}
