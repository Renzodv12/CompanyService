using MediatR;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;
using RestaurantEntity = CompanyService.Core.Entities.Restaurant.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRestaurantCommandHandler> _logger;

        public DeleteRestaurantCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRestaurantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting restaurant: {RestaurantId}", request.Id);

            var restaurants = await _unitOfWork.Repository<RestaurantEntity>()
                .WhereAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId);

            var restaurant = restaurants.FirstOrDefault();
            if (restaurant == null)
            {
                throw new ArgumentException($"Restaurant with ID {request.Id} not found");
            }

            // Check if restaurant has active orders
            var activeOrders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == request.Id && 
                                (o.Status == OrderStatus.Pending || 
                                 o.Status == OrderStatus.Confirmed || 
                                 o.Status == OrderStatus.Preparing || 
                                 o.Status == OrderStatus.Ready || 
                                 o.Status == OrderStatus.Served));

            if (activeOrders.Any())
            {
                throw new InvalidOperationException("Cannot delete restaurant with active orders");
            }

            _unitOfWork.Repository<RestaurantEntity>().Remove(restaurant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant deleted successfully: {RestaurantId}", request.Id);
            return true;
        }
    }
}
