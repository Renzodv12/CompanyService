using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class GetEntityFieldsHandler : IRequestHandler<GetEntityFieldsQuery, IEnumerable<EntityFieldDto>>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<GetEntityFieldsHandler> _logger;

        public GetEntityFieldsHandler(
            IDynamicReportService dynamicReportService,
            ILogger<GetEntityFieldsHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<IEnumerable<EntityFieldDto>> Handle(GetEntityFieldsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting entity fields for {EntityName} in company {CompanyId} by user {UserId}",
                    request.EntityName, request.CompanyId, request.UserId);

                var result = await _dynamicReportService.GetEntityFieldsAsync(request.EntityName, request.CompanyId);

                _logger.LogInformation("Successfully retrieved {Count} fields for entity {EntityName}",
                    result.Count(), request.EntityName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity fields for {EntityName} in company {CompanyId}",
                    request.EntityName, request.CompanyId);
                throw;
            }
        }
    }
}