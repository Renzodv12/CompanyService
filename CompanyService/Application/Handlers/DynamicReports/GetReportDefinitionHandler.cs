using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class GetReportDefinitionHandler : IRequestHandler<GetReportDefinitionQuery, ReportDefinitionResponseDto?>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<GetReportDefinitionHandler> _logger;

        public GetReportDefinitionHandler(
            IDynamicReportService dynamicReportService,
            ILogger<GetReportDefinitionHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<ReportDefinitionResponseDto?> Handle(GetReportDefinitionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting report definition {ReportId} for company {CompanyId} by user {UserId}",
                    request.Id, request.CompanyId, request.UserId);

                // Validate user can access this report
                var canAccess = await _dynamicReportService.CanUserAccessReportAsync(
                    request.Id, 
                    request.UserId, 
                    request.CompanyId);

                if (!canAccess)
                {
                    _logger.LogWarning("User {UserId} cannot access report definition {ReportId} for company {CompanyId}",
                        request.UserId, request.Id, request.CompanyId);
                    return null;
                }

                var result = await _dynamicReportService.GetReportDefinitionAsync(request.Id, request.CompanyId);

                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved report definition {ReportId} with name {ReportName}",
                        result.Id, result.Name);
                }
                else
                {
                    _logger.LogWarning("Report definition {ReportId} not found for company {CompanyId}",
                        request.Id, request.CompanyId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report definition {ReportId} for company {CompanyId}",
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}