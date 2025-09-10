using CompanyService.Application.Commands.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class CreateReportDefinitionHandler : IRequestHandler<CreateReportDefinitionCommand, ReportDefinitionResponseDto>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<CreateReportDefinitionHandler> _logger;

        public CreateReportDefinitionHandler(
            IDynamicReportService dynamicReportService,
            ILogger<CreateReportDefinitionHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<ReportDefinitionResponseDto> Handle(CreateReportDefinitionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating report definition {ReportName} for company {CompanyId} by user {UserId}",
                    request.ReportDefinition.Name, request.CompanyId, request.UserId);

                // Validate the report definition
                var isValid = await _dynamicReportService.ValidateReportDefinitionAsync(request.ReportDefinition, request.CompanyId);
                if (!isValid)
                {
                    throw new ArgumentException("Invalid report definition.");
                }

                var result = await _dynamicReportService.CreateReportDefinitionAsync(request.ReportDefinition, request.UserId);

                _logger.LogInformation("Successfully created report definition {ReportId} with name {ReportName}",
                    result.Id, result.Name);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report definition {ReportName} for company {CompanyId}",
                    request.ReportDefinition.Name, request.CompanyId);
                throw;
            }
        }
    }
}