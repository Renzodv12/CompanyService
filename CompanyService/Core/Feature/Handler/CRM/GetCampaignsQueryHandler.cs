using MediatR;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para obtener la lista paginada de campañas
    /// </summary>
    public class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, List<CampaignDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetCampaignsQueryHandler> _logger;

        public GetCampaignsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetCampaignsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CampaignDto>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting campaigns for company {CompanyId}", 
                    request.CompanyId);

                // Obtener las campañas
                var campaigns = await _context.Campaigns
                    .Include(c => c.AssignedToUser)
                    .Where(c => c.CompanyId == request.CompanyId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CampaignDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Type = c.Type,
                        Status = c.Status,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        Budget = c.Budget,
                        ActualCost = c.ActualCost,
                        TargetAudience = c.TargetAudience,
                        ActualReach = c.ActualReach,
                        ConversionRate = c.ConversionRate,
                        ROI = c.ROI,
                        Channel = c.Channel,
                        Notes = c.Notes,
                        CompanyId = c.CompanyId,
                        AssignedUserId = c.AssignedToUserId,
                        AssignedUserName = c.AssignedToUser != null 
                            ? $"{c.AssignedToUser.FirstName} {c.AssignedToUser.LastName}".Trim()
                            : null,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        LeadsCount = c.Leads
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} campaigns for company {CompanyId}", 
                    campaigns.Count, request.CompanyId);

                return campaigns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}
