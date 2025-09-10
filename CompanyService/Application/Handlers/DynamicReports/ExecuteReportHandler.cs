using CompanyService.Application.Commands.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class ExecuteReportHandler : IRequestHandler<ExecuteReportCommand, ReportExecutionResponseDto>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<ExecuteReportHandler> _logger;

        public ExecuteReportHandler(
            IDynamicReportService dynamicReportService,
            ILogger<ExecuteReportHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<ReportExecutionResponseDto> Handle(ExecuteReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Executing report {ReportDefinitionId} for company {CompanyId} by user {UserId}",
                    request.ExecuteRequest.ReportDefinitionId, request.ExecuteRequest.CompanyId, request.UserId);

                // Validate user can execute this report
                var canExecute = await _dynamicReportService.CanUserExecuteReportAsync(
                    request.ExecuteRequest.ReportDefinitionId, 
                    request.UserId, 
                    request.ExecuteRequest.CompanyId);

                if (!canExecute)
                {
                    throw new UnauthorizedAccessException("User cannot execute this report.");
                }

                var result = await _dynamicReportService.ExecuteReportAsync(request.ExecuteRequest, request.UserId);

                _logger.LogInformation("Successfully executed report {ReportDefinitionId}, execution ID: {ExecutionId}, rows: {RowCount}",
                    request.ExecuteRequest.ReportDefinitionId, result.Id, result.RowCount);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing report {ReportDefinitionId} for company {CompanyId}",
                    request.ExecuteRequest.ReportDefinitionId, request.ExecuteRequest.CompanyId);
                throw;
            }
        }
    }
}