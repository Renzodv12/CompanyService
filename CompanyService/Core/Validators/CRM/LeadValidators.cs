using CompanyService.Core.DTOs.CRM;
using FluentValidation;

namespace CompanyService.Core.Validators.CRM
{
    public class CreateLeadDtoValidator : AbstractValidator<CreateLeadDto>
    {
        public CreateLeadDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido es requerido")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("El email es requerido")
                .EmailAddress()
                .WithMessage("El formato del email no es válido")
                .MaximumLength(255)
                .WithMessage("El email no puede exceder 255 caracteres");

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres")
                .Matches(@"^[\d\s\+\-\(\)]*$")
                .When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("El formato del teléfono no es válido");

            RuleFor(x => x.Company)
                .MaximumLength(100)
                .WithMessage("El nombre de la empresa no puede exceder 100 caracteres");

            RuleFor(x => x.JobTitle)
                .MaximumLength(100)
                .WithMessage("El título del trabajo no puede exceder 100 caracteres");

            RuleFor(x => x.Source)
                .IsInEnum()
                .WithMessage("La fuente del lead debe ser válida");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado del lead debe ser válido");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.NextFollowUpDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.NextFollowUpDate.HasValue)
                .WithMessage("La fecha de seguimiento debe ser futura");

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("El ID de la empresa es requerido");
        }
    }

    public class UpdateLeadDtoValidator : AbstractValidator<UpdateLeadDto>
    {
        public UpdateLeadDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("El formato del email no es válido")
                .MaximumLength(255)
                .WithMessage("El email no puede exceder 255 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres")
                .Matches(@"^[\d\s\+\-\(\)]*$")
                .When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("El formato del teléfono no es válido");

            RuleFor(x => x.Company)
                .MaximumLength(100)
                .WithMessage("El nombre de la empresa no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Company));

            RuleFor(x => x.JobTitle)
                .MaximumLength(100)
                .WithMessage("El título del trabajo no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.JobTitle));

            RuleFor(x => x.Source)
                .IsInEnum()
                .WithMessage("La fuente del lead debe ser válida")
                .When(x => x.Source.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado del lead debe ser válido")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.NextFollowUpDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.NextFollowUpDate.HasValue)
                .WithMessage("La fecha de seguimiento debe ser futura");
        }
    }
}