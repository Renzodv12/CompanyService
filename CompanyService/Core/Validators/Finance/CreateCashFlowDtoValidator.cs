using FluentValidation;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Finance
{
    public class CreateCashFlowDtoValidator : AbstractValidator<CreateCashFlowDto>
    {
        public CreateCashFlowDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("El tipo de flujo de caja no es válido");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a 0");

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("La fecha de transacción es requerida")
                .LessThanOrEqualTo(DateTime.Today.AddDays(1))
                .WithMessage("La fecha de transacción no puede ser futura");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres");

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(100).WithMessage("El número de referencia no puede exceder 100 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("El ID de la empresa es requerido");

            // Validación condicional: debe tener al menos una cuenta relacionada
            RuleFor(x => x)
                .Must(x => x.RelatedAccountId.HasValue || x.RelatedBankAccountId.HasValue)
                .WithMessage("Debe especificar al menos una cuenta relacionada (Account o BankAccount)");
        }
    }
}