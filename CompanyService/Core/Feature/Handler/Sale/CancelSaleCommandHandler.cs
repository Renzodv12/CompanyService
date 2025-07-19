using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Product;
using CompanyService.Core.Feature.Commands.Sale;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Sale
{
    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CancelSaleCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<bool> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener la venta
            var sale = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .FirstOrDefaultAsync(s => s.Id == request.SaleId && s.CompanyId == request.CompanyId);

            if (sale == null)
                throw new DefaultException("Venta no encontrada.");

            if (sale.Status == SaleStatus.Cancelled)
                throw new DefaultException("La venta ya está cancelada.");

            // Obtener detalles de la venta
            var saleDetails = await _unitOfWork.Repository<SaleDetail>()
                .WhereAsync(sd => sd.SaleId == request.SaleId);

            // Cancelar factura electrónica si existe
            if (sale.IsElectronicInvoice && !string.IsNullOrEmpty(sale.ElectronicInvoiceId))
            {
               // await _electronicInvoiceService.CancelElectronicInvoiceAsync(sale.ElectronicInvoiceId, request.CompanyId);
            }

            // Devolver stock
            foreach (var detail in saleDetails)
            {
                await _mediator.Send(new UpdateStockCommand
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    MovementType = MovementType.Entry,
                    Reason = $"Cancelación de venta - {request.Reason}",
                    Reference = sale.SaleNumber,
                    CompanyId = request.CompanyId,
                    UserId = request.UserId
                });
            }

            // Actualizar estado de la venta
            sale.Status = SaleStatus.Cancelled;
            sale.Notes = $"{sale.Notes}\nCancelada: {request.Reason}";
            _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Update(sale);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
