using CompanyService.Core.Models.Company;
using FluentValidation;

namespace CompanyService.Core.Validators.Company
{
    public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
    {
        public CreateCompanyRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre debe tener máximo 100 caracteres.");

            RuleFor(x => x.RUC)
                .NotEmpty().WithMessage("El RUC es obligatorio.")
                .MinimumLength(5).WithMessage("El RUC debe al menos 5 caracteres.")
                .Must(ruc => !ruc.Contains('-'))
                    .WithMessage("No incluyas el dígito verificador. Ejemplo válido: 1234567");
        }
    }
}
