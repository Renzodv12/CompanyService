using CompanyService.Core.DTOs.CRM;
using FluentValidation;

namespace CompanyService.Core.Validators.CRM
{
    public class CreateCampaignDtoValidator : AbstractValidator<CreateCampaignDto>
    {
        public CreateCampaignDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre de la campaña es requerido")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder 1000 caracteres");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("El tipo de campaña debe ser válido");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado de la campaña debe ser válido");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("La fecha de inicio es requerida")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("La fecha de inicio debe ser hoy o en el futuro");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio");

            RuleFor(x => x.Budget)
                .GreaterThan(0)
                .WithMessage("El presupuesto debe ser mayor a 0")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("El presupuesto no puede exceder 999,999,999.99");

            RuleFor(x => x.TargetAudience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La audiencia objetivo debe ser mayor o igual a 0");

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("El ID de la empresa es requerido");
        }
    }

    public class UpdateCampaignDtoValidator : AbstractValidator<UpdateCampaignDto>
    {
        public UpdateCampaignDtoValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("La descripción no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("El tipo de campaña debe ser válido")
                .When(x => x.Type.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado de la campaña debe ser válido")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("La fecha de inicio debe ser hoy o en el futuro")
                .When(x => x.StartDate.HasValue);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue && x.StartDate.HasValue)
                .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio");

            RuleFor(x => x.Budget)
                .GreaterThan(0)
                .WithMessage("El presupuesto debe ser mayor a 0")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("El presupuesto no puede exceder 999,999,999.99")
                .When(x => x.Budget.HasValue);

            RuleFor(x => x.TargetAudience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La audiencia objetivo debe ser mayor o igual a 0")
                .When(x => x.TargetAudience.HasValue);
        }
    }
}