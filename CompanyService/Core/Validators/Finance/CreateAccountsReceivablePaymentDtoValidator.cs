using FluentValidation;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Finance
{
    public class CreateAccountsReceivablePaymentDtoValidator : AbstractValidator<CreateAccountsReceivablePaymentDto>
    {
        public CreateAccountsReceivablePaymentDtoValidator()
        {
            RuleFor(x => x.AccountsReceivableId)
                .NotEmpty().WithMessage("El ID de la cuenta por cobrar es requerido");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto del pago debe ser mayor a 0");

            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("La fecha de pago es requerida")
                .LessThanOrEqualTo(DateTime.Today.AddDays(1))
                .WithMessage("La fecha de pago no puede ser futura");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("El método de pago no es válido");

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(100).WithMessage("El número de referencia no puede exceder 100 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("El ID de la empresa es requerido");
        }
    }
}