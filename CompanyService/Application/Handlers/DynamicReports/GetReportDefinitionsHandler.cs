using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class GetReportDefinitionsHandler : IRequestHandler<GetReportDefinitionsQuery, IEnumerable<ReportDefinitionListDto>>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<GetReportDefinitionsHandler> _logger;

        public GetReportDefinitionsHandler(
            IDynamicReportService dynamicReportService,
            ILogger<GetReportDefinitionsHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<IEnumerable<ReportDefinitionListDto>> Handle(GetReportDefinitionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting report definitions for company {CompanyId} by user {UserId}, includeShared: {IncludeShared}",
                    request.CompanyId, request.UserId, request.IncludeShared);

                var result = await _dynamicReportService.GetReportDefinitionsAsync(
                    request.CompanyId, 
                    request.IncludeShared);

                _logger.LogInformation("Successfully retrieved {Count} report definitions for company {CompanyId}",
                    result.Count(), request.CompanyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report definitions for company {CompanyId}",
                    request.CompanyId);
                throw;
            }
        }
    }
}