using CompanyService.Application.Commands.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class UpdateReportDefinitionHandler : IRequestHandler<UpdateReportDefinitionCommand, ReportDefinitionResponseDto>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<UpdateReportDefinitionHandler> _logger;

        public UpdateReportDefinitionHandler(
            IDynamicReportService dynamicReportService,
            ILogger<UpdateReportDefinitionHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<ReportDefinitionResponseDto> Handle(UpdateReportDefinitionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating report definition {ReportId} for company {CompanyId} by user {UserId}",
                    request.Id, request.CompanyId, request.UserId);

                var result = await _dynamicReportService.UpdateReportDefinitionAsync(
                    request.Id, 
                    request.ReportDefinition, 
                    request.UserId, 
                    request.CompanyId);

                _logger.LogInformation("Successfully updated report definition {ReportId} with name {ReportName}",
                    result.Id, result.Name);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report definition {ReportId} for company {CompanyId}",
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}