using MediatR;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Commands.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class ChangeTableStatusCommandHandler : IRequestHandler<ChangeTableStatusCommand, RestaurantTableDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChangeTableStatusCommandHandler> _logger;

        public ChangeTableStatusCommandHandler(IUnitOfWork unitOfWork, ILogger<ChangeTableStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantTableDto> Handle(ChangeTableStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing table status: {TableId} to {Status}", request.Id, request.Status);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId);

            var table = tables.FirstOrDefault();
            if (table == null)
            {
                throw new ArgumentException($"Restaurant table with ID {request.Id} not found");
            }

            table.Status = (TableStatus)request.Status;
            table.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantTable>().Update(table);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Table status changed successfully: {TableId} to {Status}", table.Id, table.Status);

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
