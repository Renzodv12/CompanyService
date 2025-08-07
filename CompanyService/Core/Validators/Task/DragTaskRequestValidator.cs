using CompanyService.Core.Models.Task;
using FluentValidation;

namespace CompanyService.Core.Validators.Task
{
    public class DragTaskRequestValidator : AbstractValidator<DragTaskRequest>
    {
        public DragTaskRequestValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("El ID de la tarea es obligatorio.");

            RuleFor(x => x.SourceColumnId)
                .NotEmpty().WithMessage("El ID de la columna origen es obligatorio.");

            RuleFor(x => x.TargetColumnId)
                .NotEmpty().WithMessage("El ID de la columna destino es obligatorio.");

            RuleFor(x => x.NewPosition)
                .GreaterThanOrEqualTo(0).WithMessage("La nueva posición debe ser mayor o igual a 0.");
        }
    }

}
