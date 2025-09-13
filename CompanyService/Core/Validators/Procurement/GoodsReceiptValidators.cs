using FluentValidation;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Enums;

namespace CompanyService.Core.Validators.Procurement
{
    public class CreateGoodsReceiptRequestValidator : AbstractValidator<CreateGoodsReceiptRequest>
    {
        public CreateGoodsReceiptRequestValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0)
                .WithMessage("CompanyId debe ser mayor a 0");

            RuleFor(x => x.PurchaseOrderId)
                .GreaterThan(0)
                .WithMessage("PurchaseOrderId debe ser mayor a 0");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0)
                .WithMessage("SupplierId debe ser mayor a 0");

            RuleFor(x => x.ReceiptNumber)
                .NotEmpty()
                .WithMessage("El número de recepción es requerido")
                .MaximumLength(50)
                .WithMessage("El número de recepción no puede exceder 50 caracteres");

            RuleFor(x => x.ReceiptDate)
                .NotEmpty()
                .WithMessage("La fecha de recepción es requerida")
                .LessThanOrEqualTo(DateTime.Today.AddDays(1))
                .WithMessage("La fecha de recepción no puede ser en el futuro");

            RuleFor(x => x.DeliveryNote)
                .MaximumLength(100)
                .WithMessage("La nota de entrega no puede exceder 100 caracteres");

            RuleFor(x => x.InvoiceNumber)
                .MaximumLength(50)
                .WithMessage("El número de factura no puede exceder 50 caracteres");

            RuleFor(x => x.TransportCompany)
                .MaximumLength(200)
                .WithMessage("La empresa de transporte no puede exceder 200 caracteres");

            RuleFor(x => x.VehicleInfo)
                .MaximumLength(200)
                .WithMessage("La información del vehículo no puede exceder 200 caracteres");

            RuleFor(x => x.DriverInfo)
                .MaximumLength(200)
                .WithMessage("La información del conductor no puede exceder 200 caracteres");

            RuleFor(x => x.FreightCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.FreightCost.HasValue)
                .WithMessage("El costo de flete debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El costo de flete no puede exceder 999,999,999");

            RuleFor(x => x.InsuranceCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.InsuranceCost.HasValue)
                .WithMessage("El costo de seguro debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El costo de seguro no puede exceder 999,999,999");

            RuleFor(x => x.OtherCosts)
                .GreaterThanOrEqualTo(0)
                .When(x => x.OtherCosts.HasValue)
                .WithMessage("Otros costos deben ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("Otros costos no pueden exceder 999,999,999");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Debe incluir al menos un item")
                .Must(items => items.Count <= 100)
                .WithMessage("No puede incluir más de 100 items");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateGoodsReceiptItemRequestValidator());
        }
    }

    public class CreateGoodsReceiptItemRequestValidator : AbstractValidator<CreateGoodsReceiptItemRequest>
    {
        public CreateGoodsReceiptItemRequestValidator()
        {
            RuleFor(x => x.PurchaseOrderItemId)
                .NotEmpty()
                .WithMessage("PurchaseOrderItemId debe ser mayor a 0");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId debe ser mayor a 0");

            RuleFor(x => x.OrderedQuantity)
                .GreaterThan(0)
                .WithMessage("La cantidad ordenada debe ser mayor a 0")
                .LessThanOrEqualTo(999999)
                .WithMessage("La cantidad ordenada no puede exceder 999,999");

            RuleFor(x => x.ReceivedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad recibida debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999)
                .WithMessage("La cantidad recibida no puede exceder 999,999");

            RuleFor(x => x.AcceptedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad aceptada debe ser mayor o igual a 0")
                .LessThanOrEqualTo(x => x.ReceivedQuantity)
                .WithMessage("La cantidad aceptada no puede exceder la cantidad recibida");

            RuleFor(x => x.RejectedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad rechazada debe ser mayor o igual a 0")
                .LessThanOrEqualTo(x => x.ReceivedQuantity)
                .WithMessage("La cantidad rechazada no puede exceder la cantidad recibida");

            RuleFor(x => x.DamagedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad dañada debe ser mayor o igual a 0")
                .LessThanOrEqualTo(x => x.ReceivedQuantity)
                .WithMessage("La cantidad dañada no puede exceder la cantidad recibida");

            RuleFor(x => x)
                .Must(x => x.AcceptedQuantity + x.RejectedQuantity + x.DamagedQuantity == x.ReceivedQuantity)
                .WithMessage("La suma de cantidades aceptadas, rechazadas y dañadas debe igual a la cantidad recibida");

            RuleFor(x => x.QualityStatus)
                .IsInEnum()
                .WithMessage("El estado de calidad debe ser un valor válido");

            RuleFor(x => x.BatchNumber)
                .MaximumLength(50)
                .WithMessage("El número de lote no puede exceder 50 caracteres");

            RuleFor(x => x.SerialNumbers)
                .MaximumLength(1000)
                .WithMessage("Los números de serie no pueden exceder 1000 caracteres");

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.ExpirationDate.HasValue)
                .WithMessage("La fecha de vencimiento debe ser en el futuro");

            RuleFor(x => x.ManufactureDate)
                .LessThanOrEqualTo(DateTime.Today)
                .When(x => x.ManufactureDate.HasValue)
                .WithMessage("La fecha de fabricación no puede ser en el futuro");

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(x => x.ManufactureDate)
                .When(x => x.ExpirationDate.HasValue && x.ManufactureDate.HasValue)
                .WithMessage("La fecha de vencimiento debe ser mayor a la fecha de fabricación");

            RuleFor(x => x.StorageLocation)
                .MaximumLength(100)
                .WithMessage("La ubicación de almacenamiento no puede exceder 100 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }

    public class UpdateGoodsReceiptRequestValidator : AbstractValidator<UpdateGoodsReceiptRequest>
    {
        public UpdateGoodsReceiptRequestValidator()
        {
            RuleFor(x => x.ReceiptDate)
                .NotEmpty()
                .WithMessage("La fecha de recepción es requerida")
                .LessThanOrEqualTo(DateTime.Today.AddDays(1))
                .WithMessage("La fecha de recepción no puede ser en el futuro");

            RuleFor(x => x.DeliveryNote)
                .MaximumLength(100)
                .WithMessage("La nota de entrega no puede exceder 100 caracteres");

            RuleFor(x => x.InvoiceNumber)
                .MaximumLength(50)
                .WithMessage("El número de factura no puede exceder 50 caracteres");

            RuleFor(x => x.TransportCompany)
                .MaximumLength(200)
                .WithMessage("La empresa de transporte no puede exceder 200 caracteres");

            RuleFor(x => x.VehicleInfo)
                .MaximumLength(200)
                .WithMessage("La información del vehículo no puede exceder 200 caracteres");

            RuleFor(x => x.DriverInfo)
                .MaximumLength(200)
                .WithMessage("La información del conductor no puede exceder 200 caracteres");

            RuleFor(x => x.FreightCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.FreightCost.HasValue)
                .WithMessage("El costo de flete debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El costo de flete no puede exceder 999,999,999");

            RuleFor(x => x.InsuranceCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.InsuranceCost.HasValue)
                .WithMessage("El costo de seguro debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("El costo de seguro no puede exceder 999,999,999");

            RuleFor(x => x.OtherCosts)
                .GreaterThanOrEqualTo(0)
                .When(x => x.OtherCosts.HasValue)
                .WithMessage("Otros costos deben ser mayor o igual a 0")
                .LessThanOrEqualTo(999999999)
                .WithMessage("Otros costos no pueden exceder 999,999,999");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Debe incluir al menos un item")
                .Must(items => items.Count <= 100)
                .WithMessage("No puede incluir más de 100 items");

            RuleForEach(x => x.Items)
                .SetValidator(new UpdateGoodsReceiptItemRequestValidator());
        }
    }

    public class UpdateGoodsReceiptItemRequestValidator : AbstractValidator<UpdateGoodsReceiptItemRequest>
    {
        public UpdateGoodsReceiptItemRequestValidator()
        {
            RuleFor(x => x.PurchaseOrderItemId)
                .NotEmpty()
                .WithMessage("PurchaseOrderItemId debe ser mayor a 0");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId debe ser mayor a 0");

            RuleFor(x => x.ReceivedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad recibida debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999)
                .WithMessage("La cantidad recibida no puede exceder 999,999");

            RuleFor(x => x.AcceptedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad aceptada debe ser mayor o igual a 0")
                .LessThanOrEqualTo(x => x.ReceivedQuantity)
                .WithMessage("La cantidad aceptada no puede exceder la cantidad recibida");

            RuleFor(x => x.RejectedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad rechazada debe ser mayor o igual a 0")
                .LessThanOrEqualTo(x => x.ReceivedQuantity)
                .WithMessage("La cantidad rechazada no puede exceder la cantidad recibida");

            RuleFor(x => x.DamagedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad dañada debe ser mayor o igual a 0")
                .LessThanOrEqualTo(x => x.ReceivedQuantity)
                .WithMessage("La cantidad dañada no puede exceder la cantidad recibida");

            RuleFor(x => x)
                .Must(x => x.AcceptedQuantity + x.RejectedQuantity + x.DamagedQuantity == x.ReceivedQuantity)
                .WithMessage("La suma de cantidades aceptadas, rechazadas y dañadas debe igual a la cantidad recibida");

            RuleFor(x => x.QualityStatus)
                .IsInEnum()
                .WithMessage("El estado de calidad debe ser un valor válido");

            RuleFor(x => x.BatchNumber)
                .MaximumLength(50)
                .WithMessage("El número de lote no puede exceder 50 caracteres");

            RuleFor(x => x.SerialNumbers)
                .MaximumLength(1000)
                .WithMessage("Los números de serie no pueden exceder 1000 caracteres");

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.ExpirationDate.HasValue)
                .WithMessage("La fecha de vencimiento debe ser en el futuro");

            RuleFor(x => x.ManufactureDate)
                .LessThanOrEqualTo(DateTime.Today)
                .When(x => x.ManufactureDate.HasValue)
                .WithMessage("La fecha de fabricación no puede ser en el futuro");

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(x => x.ManufactureDate)
                .When(x => x.ExpirationDate.HasValue && x.ManufactureDate.HasValue)
                .WithMessage("La fecha de vencimiento debe ser mayor a la fecha de fabricación");

            RuleFor(x => x.StorageLocation)
                .MaximumLength(100)
                .WithMessage("La ubicación de almacenamiento no puede exceder 100 caracteres");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        }
    }

    public class UpdateGoodsReceiptStatusRequestValidator : AbstractValidator<UpdateGoodsReceiptStatusRequest>
    {
        public UpdateGoodsReceiptStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("El estado debe ser un valor válido");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres");
        }
    }

    public class InspectGoodsReceiptItemRequestValidator : AbstractValidator<InspectGoodsReceiptItemRequest>
    {
        public InspectGoodsReceiptItemRequestValidator()
        {
            RuleFor(x => x.GoodsReceiptItemId)
                .NotEmpty()
                .WithMessage("GoodsReceiptItemId debe ser mayor a 0");

            RuleFor(x => x.QualityStatus)
                .IsInEnum()
                .WithMessage("El estado de calidad debe ser un valor válido");

            RuleFor(x => x.AcceptedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad aceptada debe ser mayor o igual a 0");

            RuleFor(x => x.RejectedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad rechazada debe ser mayor o igual a 0");

            RuleFor(x => x.DamagedQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad dañada debe ser mayor o igual a 0");

            RuleFor(x => x.InspectionNotes)
                .MaximumLength(1000)
                .WithMessage("Las notas de inspección no pueden exceder 1000 caracteres");

            RuleFor(x => x.DefectPhotos)
                .Must(photos => photos == null || photos.Count <= 10)
                .WithMessage("No puede incluir más de 10 fotos de defectos");
        }
    }

    public class BulkInspectionRequestValidator : AbstractValidator<BulkInspectionRequest>
    {
        public BulkInspectionRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Debe incluir al menos un item para inspeccionar")
                .Must(items => items.Count <= 50)
                .WithMessage("No puede inspeccionar más de 50 items a la vez");

            RuleForEach(x => x.Items)
                .SetValidator(new InspectGoodsReceiptItemRequestValidator());

            RuleFor(x => x.GeneralNotes)
                .MaximumLength(1000)
                .WithMessage("Las notas generales no pueden exceder 1000 caracteres");
        }
    }

    public class GoodsReceiptFilterRequestValidator : AbstractValidator<GoodsReceiptFilterRequest>
    {
        public GoodsReceiptFilterRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("El estado debe ser un valor válido");

            RuleFor(x => x.QualityStatus)
                .IsInEnum()
                .When(x => x.QualityStatus.HasValue)
                .WithMessage("El estado de calidad debe ser un valor válido");

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

    public class GoodsReceiptReportRequestValidator : AbstractValidator<GoodsReceiptReportRequest>
    {
        public GoodsReceiptReportRequestValidator()
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
        }
    }
}