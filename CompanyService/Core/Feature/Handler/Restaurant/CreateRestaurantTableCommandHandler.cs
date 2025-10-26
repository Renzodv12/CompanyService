using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class CreateRestaurantTableCommandHandler : IRequestHandler<CreateRestaurantTableCommand, RestaurantTableDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRestaurantTableCommandHandler> _logger;

        public CreateRestaurantTableCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRestaurantTableCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantTableDto> Handle(CreateRestaurantTableCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating restaurant table: {TableNumber} for restaurant {RestaurantId}", request.TableNumber, request.RestaurantId);

            var table = new RestaurantTable
            {
                Id = Guid.NewGuid(),
                RestaurantId = request.RestaurantId,
                TableNumber = request.TableNumber,
                Capacity = request.Capacity,
                Location = request.Location,
                Status = TableStatus.Available,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = request.CompanyId,
                CreatedBy = request.UserId
            };

            await _unitOfWork.Repository<RestaurantTable>().AddAsync(table);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant table created successfully with ID: {TableId}", table.Id);

            return new RestaurantTableDto
            {
                Id = table.Id,
                RestaurantId = table.RestaurantId,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                Location = table.Location,
                Status = table.Status.ToString(),
                IsActive = table.IsActive,
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt,
                RestaurantName = "System" // TODO: Get actual restaurant name
            };
        }
    }
}
