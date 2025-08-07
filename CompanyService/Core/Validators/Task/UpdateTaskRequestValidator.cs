using CompanyService.Core.Models.Task;
using FluentValidation;

namespace CompanyService.Core.Validators.Task
{
    public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
    {
        public UpdateTaskRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título de la tarea es obligatorio.")
                .MaximumLength(200).WithMessage("El título debe tener máximo 200 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("La descripción debe tener máximo 2000 caracteres.");

            RuleFor(x => x.Labels)
                .Must(labels => labels == null || labels.All(l => !string.IsNullOrWhiteSpace(l)))
                .WithMessage("Las etiquetas no pueden estar vacías.");

            RuleForEach(x => x.Labels)
                .MaximumLength(50).WithMessage("Cada etiqueta debe tener máximo 50 caracteres.");
        }
    }
}
