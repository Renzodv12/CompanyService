using FluentValidation;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Finance
{
    public class CreateBankTransactionDtoValidator : AbstractValidator<CreateBankTransactionDto>
    {
        public CreateBankTransactionDtoValidator()
        {
            RuleFor(x => x.BankAccountId)
                .NotEmpty().WithMessage("El ID de la cuenta bancaria es requerido");

            RuleFor(x => x.TransactionNumber)
                .NotEmpty().WithMessage("El número de transacción es requerido")
                .MaximumLength(50).WithMessage("El número de transacción no puede exceder 50 caracteres");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("El tipo de transacción no es válido");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a 0");

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("La fecha de transacción es requerida")
                .LessThanOrEqualTo(DateTime.Today.AddDays(1))
                .WithMessage("La fecha de transacción no puede ser futura");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres");

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(100).WithMessage("El número de referencia no puede exceder 100 caracteres");

            RuleFor(x => x.Payee)
                .MaximumLength(200).WithMessage("El beneficiario no puede exceder 200 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("El ID de la empresa es requerido");
        }
    }
}