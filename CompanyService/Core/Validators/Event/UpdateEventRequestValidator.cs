using CompanyService.Core.Models.Event;
using FluentValidation;

namespace CompanyService.Core.Validators.Event
{
    public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
    {
        public UpdateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título del evento es obligatorio.")
                .MaximumLength(200).WithMessage("El título debe tener máximo 200 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción debe tener máximo 1000 caracteres.");

            RuleFor(x => x.Start)
                .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

            RuleFor(x => x.End)
                .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
                .GreaterThan(x => x.Start).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

            RuleFor(x => x.AttendeeUserIds)
                .Must(ids => ids == null || ids.Count <= 50)
                .WithMessage("Un evento no puede tener más de 50 asistentes.");
        }
    }
}
