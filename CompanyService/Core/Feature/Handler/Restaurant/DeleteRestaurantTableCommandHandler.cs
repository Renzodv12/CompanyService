using MediatR;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class DeleteRestaurantTableCommandHandler : IRequestHandler<DeleteRestaurantTableCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRestaurantTableCommandHandler> _logger;

        public DeleteRestaurantTableCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRestaurantTableCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRestaurantTableCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting restaurant table: {TableId}", request.Id);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId);

            var table = tables.FirstOrDefault();
            if (table == null)
            {
                throw new ArgumentException($"Restaurant table with ID {request.Id} not found");
            }

            // Check if table has active orders
            var activeOrders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.TableId == request.Id && 
                                (o.Status == OrderStatus.Pending || 
                                 o.Status == OrderStatus.Confirmed || 
                                 o.Status == OrderStatus.Preparing || 
                                 o.Status == OrderStatus.Ready || 
                                 o.Status == OrderStatus.Served));

            if (activeOrders.Any())
            {
                throw new InvalidOperationException("Cannot delete table with active orders");
            }

            _unitOfWork.Repository<RestaurantTable>().Remove(table);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant table deleted successfully: {TableId}", request.Id);
            return true;
        }
    }
}
