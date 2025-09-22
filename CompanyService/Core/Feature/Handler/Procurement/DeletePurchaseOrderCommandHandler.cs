using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para eliminar una orden de compra
    /// </summary>
    public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            // Buscar la orden de compra existente
            var purchaseOrder = await _unitOfWork.Repository<PurchaseOrder>()
                .GetByIdAsync(request.Id);

            if (purchaseOrder == null || purchaseOrder.CompanyId != request.CompanyId)
                throw new DefaultException("Orden de compra no encontrada.");

            // Eliminar la orden de compra
            _unitOfWork.Repository<PurchaseOrder>().Remove(purchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}