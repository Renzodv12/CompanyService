using CompanyService.Core.DTOs.CRM;
using FluentValidation;

namespace CompanyService.Core.Validators.CRM
{
    public class CreateOpportunityDtoValidator : AbstractValidator<CreateOpportunityDto>
    {
        public CreateOpportunityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre de la oportunidad es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder 1000 caracteres");

            RuleFor(x => x.Value)
                .GreaterThan(0)
                .WithMessage("El valor debe ser mayor a 0")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("El valor no puede exceder 999,999,999.99");

            RuleFor(x => x.Stage)
                .IsInEnum()
                .WithMessage("La etapa de la oportunidad debe ser válida");

            RuleFor(x => x.Probability)
                .InclusiveBetween(0, 100)
                .WithMessage("La probabilidad debe estar entre 0 y 100");

            RuleFor(x => x.ExpectedCloseDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .When(x => x.ExpectedCloseDate.HasValue)
                .WithMessage("La fecha esperada de cierre debe ser futura");



            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("El ID de la empresa es requerido");
        }
    }

    public class UpdateOpportunityDtoValidator : AbstractValidator<UpdateOpportunityDto>
    {
        public UpdateOpportunityDtoValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Value)
                .GreaterThan(0)
                .WithMessage("El valor debe ser mayor a 0")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("El valor no puede exceder 999,999,999.99")
                .When(x => x.Value.HasValue);

            RuleFor(x => x.Stage)
                .IsInEnum()
                .WithMessage("La etapa de la oportunidad debe ser válida")
                .When(x => x.Stage.HasValue);

            RuleFor(x => x.Probability)
                .InclusiveBetween(0, 100)
                .WithMessage("La probabilidad debe estar entre 0 y 100");

            RuleFor(x => x.ExpectedCloseDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .When(x => x.ExpectedCloseDate.HasValue)
                .WithMessage("La fecha esperada de cierre debe ser futura");
        }
    }

    public class OpportunityStageUpdateDtoValidator : AbstractValidator<OpportunityStageUpdateDto>
    {
        public OpportunityStageUpdateDtoValidator()
        {
            RuleFor(x => x.Stage)
                .IsInEnum()
                .WithMessage("La etapa de la oportunidad debe ser válida");

            RuleFor(x => x.Probability)
                .InclusiveBetween(0, 100)
                .WithMessage("La probabilidad debe estar entre 0 y 100");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}