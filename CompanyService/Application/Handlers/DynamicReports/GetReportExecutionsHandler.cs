using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class GetReportExecutionsHandler : IRequestHandler<GetReportExecutionsQuery, IEnumerable<ReportExecutionListDto>>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<GetReportExecutionsHandler> _logger;

        public GetReportExecutionsHandler(
            IDynamicReportService dynamicReportService,
            ILogger<GetReportExecutionsHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<IEnumerable<ReportExecutionListDto>> Handle(GetReportExecutionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting report executions for company {CompanyId} by user {UserId}, page {PageNumber}, size {PageSize}",
                    request.CompanyId, request.UserId, request.PageNumber, request.PageSize);

                var result = await _dynamicReportService.GetReportExecutionsAsync(
                    request.CompanyId, 
                    request.FilterByUserId, 
                    request.PageNumber, 
                    request.PageSize);

                _logger.LogInformation("Successfully retrieved {Count} report executions for company {CompanyId}",
                    result.Count(), request.CompanyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report executions for company {CompanyId}",
                    request.CompanyId);
                throw;
            }
        }
    }
}