using FluentValidation;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Validators.Finance
{
    public class CreateAccountsPayableDtoValidator : AbstractValidator<CreateAccountsPayableDto>
    {
        public CreateAccountsPayableDtoValidator()
        {
            RuleFor(x => x.InvoiceNumber)
                .NotEmpty().WithMessage("El número de factura es requerido")
                .MaximumLength(50).WithMessage("El número de factura no puede exceder 50 caracteres");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("El ID del proveedor es requerido");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("El monto total debe ser mayor a 0");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("La fecha de vencimiento es requerida")
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30))
                .WithMessage("La fecha de vencimiento no puede ser anterior a 30 días");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("El ID de la empresa es requerido");
        }
    }
}