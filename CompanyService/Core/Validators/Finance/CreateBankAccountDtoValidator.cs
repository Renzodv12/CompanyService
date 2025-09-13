using FluentValidation;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Validators.Finance
{
    public class CreateBankAccountDtoValidator : AbstractValidator<CreateBankAccountDto>
    {
        public CreateBankAccountDtoValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("El número de cuenta es requerido")
                .MaximumLength(50).WithMessage("El número de cuenta no puede exceder 50 caracteres");

            RuleFor(x => x.BankName)
                .NotEmpty().WithMessage("El nombre del banco es requerido")
                .MaximumLength(100).WithMessage("El nombre del banco no puede exceder 100 caracteres");

            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("El nombre de la cuenta es requerido")
                .MaximumLength(100).WithMessage("El nombre de la cuenta no puede exceder 100 caracteres");

            RuleFor(x => x.AccountType)
                .MaximumLength(20).WithMessage("El tipo de cuenta no puede exceder 20 caracteres");

            RuleFor(x => x.SwiftCode)
                .MaximumLength(20).WithMessage("El código SWIFT no puede exceder 20 caracteres");

            RuleFor(x => x.RoutingNumber)
                .MaximumLength(50).WithMessage("El número de ruta no puede exceder 50 caracteres");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0).WithMessage("El saldo inicial no puede ser negativo");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("La moneda es requerida")
                .Length(3).WithMessage("La moneda debe tener exactamente 3 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("El ID de la empresa es requerido");
        }
    }
}