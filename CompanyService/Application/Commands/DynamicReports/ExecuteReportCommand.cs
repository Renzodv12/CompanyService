using CompanyService.Core.DTOs.DynamicReports;
using MediatR;

namespace CompanyService.Application.Commands.DynamicReports
{
    public class ExecuteReportCommand : IRequest<ReportExecutionResponseDto>
    {
        public ExecuteReportDto ExecuteRequest { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}