using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class DeleteRestaurantOrderCommandHandler : IRequestHandler<DeleteRestaurantOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRestaurantOrderCommandHandler> _logger;

        public DeleteRestaurantOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRestaurantOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRestaurantOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting restaurant order: {OrderId}", request.Id);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == request.Id && o.RestaurantId == request.RestaurantId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {request.Id} not found");
            }

            // Check if order can be deleted (only pending orders can be deleted)
            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be deleted");
            }

            // Update table status to available
            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == order.TableId);

            var table = tables.FirstOrDefault();
            if (table != null)
            {
                table.Status = TableStatus.Available;
                table.CurrentOrderId = null;
                table.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<RestaurantTable>().Update(table);
            }

            // Delete associated sale if exists
            var sales = await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>()
                .WhereAsync(s => s.Id == order.SaleId);

            var sale = sales.FirstOrDefault();
            if (sale != null)
            {
                _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().Remove(sale);
            }

            _unitOfWork.Repository<RestaurantOrder>().Remove(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant order deleted successfully: {OrderId}", request.Id);
            return true;
        }
    }
}
