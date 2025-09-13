using FluentValidation;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Validators.Finance
{
    public class CreateBudgetDtoValidator : AbstractValidator<CreateBudgetDto>
    {
        public CreateBudgetDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del presupuesto es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.Year)
                .InclusiveBetween(2020, 2050).WithMessage("El año debe estar entre 2020 y 2050");

            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("El mes debe estar entre 1 y 12")
                .When(x => x.Month.HasValue);

            RuleFor(x => x.BudgetedAmount)
                .GreaterThanOrEqualTo(0).WithMessage("El monto presupuestado no puede ser negativo");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("El ID de la empresa es requerido");

            RuleForEach(x => x.BudgetLines)
                .SetValidator(new CreateBudgetLineDtoValidator());
        }
    }

    public class CreateBudgetLineDtoValidator : AbstractValidator<CreateBudgetLineDto>
    {
        public CreateBudgetLineDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción de la línea de presupuesto es requerida")
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres");

            RuleFor(x => x.BudgetedAmount)
                .GreaterThanOrEqualTo(0).WithMessage("El monto presupuestado no puede ser negativo");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }
}