using CompanyService.Application.Commands.DynamicReports;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyService.Application.Handlers.DynamicReports
{
    public class DeleteReportDefinitionHandler : IRequestHandler<DeleteReportDefinitionCommand, bool>
    {
        private readonly IDynamicReportService _dynamicReportService;
        private readonly ILogger<DeleteReportDefinitionHandler> _logger;

        public DeleteReportDefinitionHandler(
            IDynamicReportService dynamicReportService,
            ILogger<DeleteReportDefinitionHandler> logger)
        {
            _dynamicReportService = dynamicReportService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteReportDefinitionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting report definition {ReportId} for company {CompanyId} by user {UserId}",
                    request.Id, request.CompanyId, request.UserId);

                var result = await _dynamicReportService.DeleteReportDefinitionAsync(
                    request.Id, 
                    request.UserId, 
                    request.CompanyId);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted report definition {ReportId}", request.Id);
                }
                else
                {
                    _logger.LogWarning("Report definition {ReportId} not found or could not be deleted", request.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report definition {ReportId} for company {CompanyId}",
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}