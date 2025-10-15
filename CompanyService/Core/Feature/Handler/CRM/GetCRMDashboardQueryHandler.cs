using MediatR;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.DTOs.CRM;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para obtener el dashboard de CRM
    /// </summary>
    public class GetCRMDashboardQueryHandler : IRequestHandler<GetCRMDashboardQuery, CRMDashboardDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetCRMDashboardQueryHandler> _logger;

        public GetCRMDashboardQueryHandler(
            ApplicationDbContext context,
            ILogger<GetCRMDashboardQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CRMDashboardDto> Handle(GetCRMDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting CRM dashboard for company {CompanyId}", request.CompanyId);

                // Obtener estadísticas de leads
                var leadStats = await GetLeadStatistics(request.CompanyId);

                // Obtener estadísticas de oportunidades
                var opportunityStats = await GetOpportunityStatistics(request.CompanyId);

                // Obtener estadísticas de campañas
                var campaignStats = await GetCampaignStatistics(request.CompanyId);

                // Obtener leads recientes
                var recentLeads = await GetRecentLeads(request.CompanyId);

                // Obtener oportunidades próximas a cerrar
                var upcomingOpportunities = await GetUpcomingOpportunities(request.CompanyId);

                // Obtener actividades recientes
                var recentActivities = await GetRecentActivities(request.CompanyId);

                var dashboard = new CRMDashboardDto
                {
                    LeadStats = leadStats,
                    OpportunityStats = opportunityStats,
                    CampaignStats = campaignStats,
                    RecentLeads = recentLeads,
                    UpcomingOpportunities = upcomingOpportunities,
                    RecentActivities = recentActivities
                };

                _logger.LogInformation("Successfully retrieved CRM dashboard for company {CompanyId}", request.CompanyId);
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CRM dashboard for company {CompanyId}", request.CompanyId);
                throw;
            }
        }

        private async Task<LeadStatistics> GetLeadStatistics(Guid companyId)
        {
            var totalLeads = await _context.Leads.CountAsync(l => l.CompanyId == companyId);

            var thisMonth = DateTime.UtcNow.AddDays(-30);
            var newLeadsThisMonth = await _context.Leads
                .CountAsync(l => l.CompanyId == companyId && l.CreatedAt >= thisMonth);

            var convertedLeadsThisMonth = await _context.Leads
                .CountAsync(l => l.CompanyId == companyId && 
                           l.Status == LeadStatus.Converted && 
                           l.CreatedAt >= thisMonth);

            var conversionRate = totalLeads > 0 
                ? (decimal)convertedLeadsThisMonth / totalLeads * 100 
                : 0;

            return new LeadStatistics
            {
                TotalLeads = totalLeads,
                NewLeadsThisMonth = newLeadsThisMonth,
                ConvertedLeadsThisMonth = convertedLeadsThisMonth,
                ConversionRate = Math.Round(conversionRate, 2)
            };
        }

        private async Task<OpportunityStatistics> GetOpportunityStatistics(Guid companyId)
        {
            var totalOpportunities = await _context.Opportunities.CountAsync(o => o.CompanyId == companyId);

            var openOpportunities = await _context.Opportunities
                .CountAsync(o => o.CompanyId == companyId && o.Stage != OpportunityStage.ClosedLost && o.Stage != OpportunityStage.ClosedWon);

            var totalOpenValue = await _context.Opportunities
                .Where(o => o.CompanyId == companyId && o.Stage != OpportunityStage.ClosedLost && o.Stage != OpportunityStage.ClosedWon)
                .SumAsync(o => o.EstimatedValue);

            var thisMonth = DateTime.UtcNow.AddDays(-30);
            var wonOpportunitiesThisMonth = await _context.Opportunities
                .CountAsync(o => o.CompanyId == companyId && 
                           o.Stage == OpportunityStage.ClosedWon && 
                           o.CloseDate >= thisMonth);

            var wonValueThisMonth = await _context.Opportunities
                .Where(o => o.CompanyId == companyId && 
                      o.Stage == OpportunityStage.ClosedWon && 
                      o.CloseDate >= thisMonth)
                .SumAsync(o => o.EstimatedValue);

            return new OpportunityStatistics
            {
                TotalOpportunities = totalOpportunities,
                OpenOpportunities = openOpportunities,
                TotalOpenValue = totalOpenValue,
                WonOpportunitiesThisMonth = wonOpportunitiesThisMonth,
                WonValueThisMonth = wonValueThisMonth
            };
        }

        private async Task<CampaignStatistics> GetCampaignStatistics(Guid companyId)
        {
            var activeCampaigns = await _context.Campaigns
                .CountAsync(c => c.CompanyId == companyId && c.Status == CampaignStatus.Active);

            var totalLeadsGenerated = await _context.CampaignLeads
                .Where(cl => cl.Campaign.CompanyId == companyId)
                .CountAsync();

            var totalCampaignCost = await _context.Campaigns
                .Where(c => c.CompanyId == companyId && c.Status == CampaignStatus.Active)
                .SumAsync(c => c.Budget);

            // Calcular ROI promedio (simplificado)
            var averageROI = activeCampaigns > 0 ? totalCampaignCost > 0 ? 150m : 0m : 0m; // TODO: Implementar cálculo real de ROI

            return new CampaignStatistics
            {
                ActiveCampaigns = activeCampaigns,
                TotalLeadsGenerated = totalLeadsGenerated,
                TotalCampaignCost = totalCampaignCost,
                AverageROI = averageROI
            };
        }

        private async Task<List<RecentLeadDto>> GetRecentLeads(Guid companyId)
        {
            var recentLeads = await _context.Leads
                .Where(l => l.CompanyId == companyId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .Select(l => new RecentLeadDto
                {
                    Id = l.Id,
                    Name = $"{l.FirstName} {l.LastName}".Trim(),
                    Email = l.Email,
                    Status = l.Status.ToString(),
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            return recentLeads;
        }

        private async Task<List<UpcomingOpportunityDto>> GetUpcomingOpportunities(Guid companyId)
        {
            var upcomingOpportunities = await _context.Opportunities
                .Where(o => o.CompanyId == companyId && 
                      o.Stage != OpportunityStage.ClosedLost && 
                      o.Stage != OpportunityStage.ClosedWon &&
                      o.ExpectedCloseDate <= DateTime.UtcNow.AddDays(30))
                .OrderBy(o => o.ExpectedCloseDate)
                .Take(5)
                .Select(o => new UpcomingOpportunityDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Value = o.EstimatedValue,
                    EstimatedCloseDate = o.ExpectedCloseDate ?? DateTime.UtcNow,
                    Probability = o.Probability
                })
                .ToListAsync();

            return upcomingOpportunities;
        }

        private async Task<List<RecentActivityDto>> GetRecentActivities(Guid companyId)
        {
            // Obtener actividades recientes de diferentes fuentes
            var recentActivities = new List<RecentActivityDto>();

            // Actividades de leads
            var recentLeads = await _context.Leads
                .Where(l => l.CompanyId == companyId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(3)
                .Select(l => new RecentActivityDto
                {
                    Id = l.Id,
                    ActivityType = "Lead Created",
                    Description = $"New lead: {l.FirstName} {l.LastName}".Trim(),
                    ActivityDate = l.CreatedAt,
                    UserName = "System" // TODO: Obtener nombre real del usuario
                })
                .ToListAsync();

            // Actividades de oportunidades
            var recentOpportunities = await _context.Opportunities
                .Where(o => o.CompanyId == companyId)
                .OrderByDescending(o => o.CreatedAt)
                .Take(3)
                .Select(o => new RecentActivityDto
                {
                    Id = o.Id,
                    ActivityType = "Opportunity Created",
                    Description = $"New opportunity: {o.Name}",
                    ActivityDate = o.CreatedAt,
                    UserName = "System" // TODO: Obtener nombre real del usuario
                })
                .ToListAsync();

            recentActivities.AddRange(recentLeads);
            recentActivities.AddRange(recentOpportunities);

            return recentActivities
                .OrderByDescending(a => a.ActivityDate)
                .Take(10)
                .ToList();
        }
    }
}
