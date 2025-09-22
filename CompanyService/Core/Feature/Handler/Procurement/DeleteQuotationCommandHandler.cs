using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para eliminar una cotización
    /// </summary>
    public class DeleteQuotationCommandHandler : IRequestHandler<DeleteQuotationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuotationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteQuotationCommand request, CancellationToken cancellationToken)
        {
            // Buscar la cotización
            var quotation = await _unitOfWork.Repository<Quotation>()
                .FirstOrDefaultAsync(q => q.Id == request.Id && q.CompanyId == request.CompanyId);

            if (quotation == null)
                throw new DefaultException("Cotización no encontrada.");

            // Verificar si la cotización puede ser eliminada (no debe estar convertida a orden de compra)
            if (quotation.ConvertedToPurchaseOrderId.HasValue)
                throw new DefaultException("No se puede eliminar una cotización que ya ha sido convertida a orden de compra.");

            // Eliminar la cotización
            _unitOfWork.Repository<Quotation>().Remove(quotation);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}