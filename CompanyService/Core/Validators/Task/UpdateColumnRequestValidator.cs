using CompanyService.Core.Models.Task;
using FluentValidation;

namespace CompanyService.Core.Validators.Task
{
    public class UpdateColumnRequestValidator : AbstractValidator<UpdateColumnRequest>
    {
        public UpdateColumnRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la columna es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre debe tener máximo 100 caracteres.");
        }
    }
}
