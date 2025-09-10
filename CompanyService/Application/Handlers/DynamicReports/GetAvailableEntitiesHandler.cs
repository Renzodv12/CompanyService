using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class GetAvailableEntitiesHandler : IRequestHandler<GetAvailableEntitiesQuery, IEnumerable<string>>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<GetAvailableEntitiesHandler> _logger;

        public GetAvailableEntitiesHandler(
            IDynamicReportService dynamicReportService,
            ILogger<GetAvailableEntitiesHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> Handle(GetAvailableEntitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting available entities for company {CompanyId} by user {UserId}",
                    request.CompanyId, request.UserId);

                var result = await _dynamicReportService.GetAvailableEntitiesAsync(request.CompanyId);

                _logger.LogInformation("Successfully retrieved {Count} available entities for company {CompanyId}",
                    result.Count(), request.CompanyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available entities for company {CompanyId}",
                    request.CompanyId);
                throw;
            }
        }
    }
}