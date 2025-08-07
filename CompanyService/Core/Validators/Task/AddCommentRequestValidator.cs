using CompanyService.Core.Models.Task;
using FluentValidation;

namespace CompanyService.Core.Validators.Task
{
    public class AddCommentRequestValidator : AbstractValidator<AddCommentRequest>
    {
        public AddCommentRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido del comentario es obligatorio.")
                .MaximumLength(2000).WithMessage("El comentario debe tener máximo 2000 caracteres.");
        }
    }

}
