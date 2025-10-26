using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class UpdateRestaurantTableCommandHandler : IRequestHandler<UpdateRestaurantTableCommand, RestaurantTableDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRestaurantTableCommandHandler> _logger;

        public UpdateRestaurantTableCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRestaurantTableCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantTableDto> Handle(UpdateRestaurantTableCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating restaurant table: {TableId}", request.Id);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId);

            var table = tables.FirstOrDefault();
            if (table == null)
            {
                throw new ArgumentException($"Restaurant table with ID {request.Id} not found");
            }

            table.TableNumber = request.TableNumber;
            table.Capacity = request.Capacity;
            table.Location = request.Location;
            table.IsActive = request.IsActive;
            table.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantTable>().Update(table);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant table updated successfully: {TableId}", table.Id);

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
