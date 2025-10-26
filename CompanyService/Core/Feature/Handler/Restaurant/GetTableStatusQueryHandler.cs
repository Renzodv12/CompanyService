using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetTableStatusQueryHandler : IRequestHandler<GetTableStatusQuery, List<TableStatusDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTableStatusQueryHandler> _logger;

        public GetTableStatusQueryHandler(IUnitOfWork unitOfWork, ILogger<GetTableStatusQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<TableStatusDto>> Handle(GetTableStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting table status for restaurant {RestaurantId}", request.RestaurantId);

            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.RestaurantId == request.RestaurantId);

            var result = tables
                .OrderBy(t => t.TableNumber)
                .Select(t => new TableStatusDto
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Name = t.Name,
                    Capacity = t.Capacity,
                    Status = t.Status.ToString(),
                    Location = t.Location,
                    IsActive = t.IsActive,
                    CurrentOrderId = t.CurrentOrderId,
                    CurrentOrderNumber = null, // TODO: Get actual order number
                    CurrentGuests = 0, // TODO: Get actual guests count
                    OrderStartTime = null, // TODO: Get actual order start time
                    EstimatedCompletionTime = null // TODO: Get actual estimated completion time
                })
                .ToList();

            return result;
        }
    }
}
