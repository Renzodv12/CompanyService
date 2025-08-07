using CompanyService.Core.Models.Task;
using FluentValidation;

namespace CompanyService.Core.Validators.Task
{
    public class UpdateSubtaskRequestValidator : AbstractValidator<UpdateSubtaskRequest>
    {
        public UpdateSubtaskRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título de la subtarea es obligatorio.")
                .MaximumLength(200).WithMessage("El título debe tener máximo 200 caracteres.");
        }
    }
}
