using FluentValidation;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Procurement
{
    public class CreateApprovalRequestValidator : AbstractValidator<CreateApprovalRequest>
    {
        public CreateApprovalRequestValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("CompanyId es requerido");

            RuleFor(x => x.EntityType)
                .NotEmpty()
                .WithMessage("El tipo de entidad es requerido")
                .MaximumLength(50)
                .WithMessage("El tipo de entidad no puede exceder 50 caracteres");

            RuleFor(x => x.EntityId)
                .NotEmpty()
                .When(x => x.EntityId.HasValue)
                .WithMessage("EntityId es requerido");

            RuleFor(x => x.RequestedByUserId)
                .NotEmpty()
                .When(x => x.RequestedByUserId.HasValue)
                .WithMessage("RequestedByUserId es requerido");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El monto debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El monto no puede exceder 999,999,999");

            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("La prioridad debe ser un valor válido");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("La razón es requerida")
                .MaximumLength(1000)
                .WithMessage("La razón no puede exceder 1000 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.RequiredDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.RequiredDate.HasValue)
                .WithMessage("La fecha requerida debe ser hoy o en el futuro");
        }
    }

    public class UpdateApprovalRequestValidator : AbstractValidator<UpdateApprovalRequest>
    {
        public UpdateApprovalRequestValidator()
        {
            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("La prioridad debe ser un valor válido");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("La razón es requerida")
                .MaximumLength(1000)
                .WithMessage("La razón no puede exceder 1000 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.RequiredDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.RequiredDate.HasValue)
                .WithMessage("La fecha requerida debe ser hoy o en el futuro");
        }
    }

    public class ProcessApprovalRequestValidator : AbstractValidator<ProcessApprovalRequest>
    {
        public ProcessApprovalRequestValidator()
        {
            RuleFor(x => x.Action)
                .IsInEnum()
                .WithMessage("La acción debe ser un valor válido");

            RuleFor(x => x.Comments)
                .NotEmpty()
                .WithMessage("Los comentarios son requeridos")
                .MaximumLength(1000)
                .WithMessage("Los comentarios no pueden exceder 1000 caracteres");

            RuleFor(x => x.DelegateToUserId)
                .NotEmpty()
                .When(x => x.Action == ApprovalAction.Delegate)
                .WithMessage("Debe especificar un usuario para delegar cuando la acción es Delegate");

            RuleFor(x => x.DelegateToUserId)
                .Null()
                .When(x => x.Action != ApprovalAction.Delegate)
                .WithMessage("No debe especificar un usuario para delegar cuando la acción no es Delegate");
        }
    }

    public class CreateApprovalLevelRequestValidator : AbstractValidator<CreateApprovalLevelRequest>
    {
        public CreateApprovalLevelRequestValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("CompanyId es requerido");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.EntityType)
                .NotEmpty()
                .WithMessage("El tipo de entidad es requerido")
                .MaximumLength(50)
                .WithMessage("El tipo de entidad no puede exceder 50 caracteres");

            RuleFor(x => x.Level)
                .GreaterThan(0)
                .WithMessage("El nivel debe ser mayor a 0")
                .LessThanOrEqualTo(10)
                .WithMessage("El nivel no puede exceder 10");

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El monto mínimo debe ser mayor o igual a 0")
                .LessThan(x => x.MaxAmount)
                .When(x => x.MaxAmount.HasValue)
                .WithMessage("El monto mínimo debe ser menor al monto máximo");

            RuleFor(x => x.MaxAmount)
                .GreaterThan(x => x.MinAmount)
                .When(x => x.MaxAmount.HasValue)
                .WithMessage("El monto máximo debe ser mayor al monto mínimo")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El monto máximo no puede exceder 999,999,999");

            RuleFor(x => x.RequiredApprovals)
                .GreaterThan(0)
                .WithMessage("El número de aprobaciones requeridas debe ser mayor a 0")
                .LessThanOrEqualTo(10)
                .WithMessage("El número de aprobaciones requeridas no puede exceder 10");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder 500 caracteres");
        }
    }

    public class UpdateApprovalLevelRequestValidator : AbstractValidator<UpdateApprovalLevelRequest>
    {
        public UpdateApprovalLevelRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.Level)
                .GreaterThan(0)
                .WithMessage("El nivel debe ser mayor a 0")
                .LessThanOrEqualTo(10)
                .WithMessage("El nivel no puede exceder 10");

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El monto mínimo debe ser mayor o igual a 0")
                .LessThan(x => x.MaxAmount)
                .When(x => x.MaxAmount.HasValue)
                .WithMessage("El monto mínimo debe ser menor al monto máximo");

            RuleFor(x => x.MaxAmount)
                .GreaterThan(x => x.MinAmount)
                .When(x => x.MaxAmount.HasValue)
                .WithMessage("El monto máximo debe ser mayor al monto mínimo")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El monto máximo no puede exceder 999,999,999");

            RuleFor(x => x.RequiredApprovals)
                .GreaterThan(0)
                .WithMessage("El número de aprobaciones requeridas debe ser mayor a 0")
                .LessThanOrEqualTo(10)
                .WithMessage("El número de aprobaciones requeridas no puede exceder 10");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder 500 caracteres");
        }
    }

    public class CreateApprovalLevelUserRequestValidator : AbstractValidator<CreateApprovalLevelUserRequest>
    {
        public CreateApprovalLevelUserRequestValidator()
        {
            RuleFor(x => x.ApprovalLevelId)
                .NotEmpty()
                .WithMessage("ApprovalLevelId es requerido");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId es requerido");

            RuleFor(x => x.CanDelegate)
                .NotNull()
                .WithMessage("Debe especificar si puede delegar");

            RuleFor(x => x.MaxDelegationLevel)
                .GreaterThan(0)
                .When(x => x.CanDelegate && x.MaxDelegationLevel.HasValue)
                .WithMessage("El nivel máximo de delegación debe ser mayor a 0")
                .LessThanOrEqualTo(10)
                .WithMessage("El nivel máximo de delegación no puede exceder 10");
        }
    }

    public class UpdateApprovalLevelUserRequestValidator : AbstractValidator<UpdateApprovalLevelUserRequest>
    {
        public UpdateApprovalLevelUserRequestValidator()
        {
            RuleFor(x => x.CanDelegate)
                .NotNull()
                .WithMessage("Debe especificar si puede delegar");

            RuleFor(x => x.MaxDelegationLevel)
                .GreaterThan(0)
                .When(x => x.CanDelegate && x.MaxDelegationLevel.HasValue)
                .WithMessage("El nivel máximo de delegación debe ser mayor a 0")
                .LessThanOrEqualTo(10)
                .WithMessage("El nivel máximo de delegación no puede exceder 10");
        }
    }

    public class ApprovalFilterRequestValidator : AbstractValidator<ApprovalFilterRequest>
    {
        public ApprovalFilterRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("El estado debe ser un valor válido");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("La fecha de inicio debe ser menor o igual a la fecha de fin");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("El término de búsqueda no puede exceder 100 caracteres");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("La página debe ser mayor a 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("El tamaño de página debe ser mayor a 0")
                .LessThanOrEqualTo(100)
                .WithMessage("El tamaño de página no puede exceder 100");
        }
    }

    public class ApprovalReportRequestValidator : AbstractValidator<ApprovalReportRequest>
    {
        public ApprovalReportRequestValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("La fecha de inicio es requerida")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("La fecha de inicio debe ser menor o igual a la fecha de fin");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("La fecha de fin es requerida")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("La fecha de fin debe ser mayor o igual a la fecha de inicio");

            RuleFor(x => x.EndDate)
                .Must((request, endDate) => (endDate - request.StartDate).TotalDays <= 365)
                .WithMessage("El rango de fechas no puede exceder 365 días");

            RuleFor(x => x.EntityType)
                .MaximumLength(50)
                .WithMessage("El tipo de entidad no puede exceder 50 caracteres");
        }
    }
}