using MediatR;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.DTOs.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para obtener un lead específico
    /// </summary>
    public class GetLeadQueryHandler : IRequestHandler<GetLeadQuery, LeadDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetLeadQueryHandler> _logger;

        public GetLeadQueryHandler(
            ApplicationDbContext context,
            ILogger<GetLeadQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LeadDto> Handle(GetLeadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting lead {LeadId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var lead = await _context.Leads
                    .Include(l => l.AssignedUser)
                    .Include(l => l.Company)
                    .FirstOrDefaultAsync(l => l.Id == request.Id && l.CompanyId == request.CompanyId, cancellationToken);

                if (lead == null)
                {
                    throw new InvalidOperationException($"Lead with ID {request.Id} not found in company {request.CompanyId}.");
                }

                var leadDto = new LeadDto
                {
                    Id = lead.Id,
                    FirstName = lead.FirstName,
                    LastName = lead.LastName,
                    Email = lead.Email,
                    Phone = lead.Phone,
                    Company = lead.CompanyName,
                    JobTitle = lead.JobTitle,
                    Source = lead.Source,
                    Status = lead.Status,
                    Notes = lead.Notes,
                    NextFollowUpDate = lead.NextFollowUpDate,
                    IsQualified = lead.IsQualified,
                    CompanyId = lead.CompanyId,
                    AssignedUserId = lead.AssignedToUserId,
                    AssignedUserName = lead.AssignedUser != null 
                        ? $"{lead.AssignedUser.FirstName} {lead.AssignedUser.LastName}".Trim()
                        : null,
                    CompanyName = lead.Company.Name,
                    CreatedAt = lead.CreatedAt,
                    UpdatedAt = lead.UpdatedAt
                };

                _logger.LogInformation("Successfully retrieved lead {LeadId}", request.Id);
                return leadDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lead {LeadId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}


