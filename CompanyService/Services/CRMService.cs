using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using CompanyService.Core.DTOs.CRM;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyService.Services
{
    public class CRMService : ICRMService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CRMService> _logger;

        public CRMService(ApplicationDbContext context, ILogger<CRMService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Lead Management

        public async Task<Lead> CreateLeadAsync(Lead lead)
        {
            try
            {
                lead.Id = Guid.NewGuid();
                lead.CreatedAt = DateTime.UtcNow;
                lead.Status = LeadStatus.New;

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lead created successfully with ID: {lead.Id}");
                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating lead: {ex.Message}");
                throw;
            }
        }

        public async Task<LeadDto> CreateLeadAsync(CreateLeadDto createLeadDto)
        {
            try
            {
                var lead = new Lead
                {
                    Id = Guid.NewGuid(),
                    CompanyId = createLeadDto.CompanyId,
                    FirstName = createLeadDto.FirstName,
                    LastName = createLeadDto.LastName,
                    Email = createLeadDto.Email,
                    Phone = createLeadDto.Phone,
                    CompanyName = createLeadDto.Company,
                    JobTitle = createLeadDto.JobTitle,
                    Source = createLeadDto.Source,
                    Status = createLeadDto.Status,
                    Score = 0, // Default score
                    Notes = createLeadDto.Notes,
                    NextFollowUpDate = createLeadDto.NextFollowUpDate,
                    IsQualified = createLeadDto.IsQualified,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Lead created successfully with ID: {lead.Id}");
                
                return new LeadDto
                {
                    Id = lead.Id,
                    CompanyId = lead.CompanyId,
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
                    CreatedAt = lead.CreatedAt,
                    UpdatedAt = lead.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating lead from DTO: {ex.Message}");
                throw;
            }
        }

        public async Task<LeadDto?> GetLeadByIdAsync(Guid leadId)
        {
            var lead = await _context.Leads
                .Include(l => l.Company)
                .Include(l => l.AssignedUser)
                .Include(l => l.CampaignLeads)
                .FirstOrDefaultAsync(l => l.Id == leadId);

            if (lead == null) return null;

            return new LeadDto
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
                AssignedUserId = lead.AssignedToUserId,
                AssignedUserName = lead.AssignedUser?.FirstName + " " + lead.AssignedUser?.LastName,
                CompanyId = lead.CompanyId,
                CreatedAt = lead.CreatedAt
            };
        }

        public async Task<IEnumerable<LeadListDto>> GetLeadsAsync(Guid companyId, int page, int pageSize)
        {
            try
            {
                var leads = await _context.Leads
                    .Where(l => l.CompanyId == companyId)
                    .OrderByDescending(l => l.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new LeadListDto
                    {
                        Id = l.Id,
                        FirstName = l.FirstName,
                        LastName = l.LastName,
                        Email = l.Email,
                        Company = l.CompanyName,
                        Status = l.Status,
                        CreatedAt = l.CreatedAt
                    })
                    .ToListAsync();

                return leads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leads for company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<Lead>> GetLeadsByCompanyAsync(Guid companyId)
        {
            return await _context.Leads
                .Include(l => l.AssignedUser)
                .Where(l => l.CompanyId == companyId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lead>> GetLeadsByStatusAsync(Guid companyId, LeadStatus status)
        {
            return await _context.Leads
                .Include(l => l.AssignedUser)
                .Where(l => l.CompanyId == companyId && l.Status == status)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<Lead> UpdateLeadAsync(Lead lead)
        {
            try
            {
                lead.UpdatedAt = DateTime.UtcNow;
                _context.Leads.Update(lead);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lead updated successfully with ID: {lead.Id}");
                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating lead: {ex.Message}");
                throw;
            }
        }

        public async Task<LeadDto?> UpdateLeadAsync(Guid id, UpdateLeadDto updateLeadDto)
        {
            try
            {
                var lead = await _context.Leads.FindAsync(id);
                if (lead == null) return null;

                lead.FirstName = updateLeadDto.FirstName;
                lead.LastName = updateLeadDto.LastName;
                lead.Email = updateLeadDto.Email;
                lead.Phone = updateLeadDto.Phone;
                if (updateLeadDto.Company != null)
                    lead.CompanyName = updateLeadDto.Company;
                if (updateLeadDto.JobTitle != null)
                    lead.JobTitle = updateLeadDto.JobTitle;
                if (updateLeadDto.Source.HasValue)
                    lead.Source = updateLeadDto.Source.Value;
                if (updateLeadDto.Status.HasValue)
                    lead.Status = updateLeadDto.Status.Value;
                if (updateLeadDto.Notes != null)
                    lead.Notes = updateLeadDto.Notes;
                if (updateLeadDto.NextFollowUpDate.HasValue)
                    lead.NextFollowUpDate = updateLeadDto.NextFollowUpDate;
                if (updateLeadDto.IsQualified.HasValue)
                    lead.IsQualified = updateLeadDto.IsQualified.Value;
                lead.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Lead updated successfully with ID: {LeadId}", id);

                return new LeadDto
                {
                    Id = lead.Id,
                    CompanyId = lead.CompanyId,
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
                    CreatedAt = lead.CreatedAt,
                    UpdatedAt = lead.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lead with ID: {LeadId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteLeadAsync(Guid leadId)
        {
            try
            {
                var lead = await _context.Leads.FindAsync(leadId);
                if (lead == null) return false;

                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lead deleted successfully with ID: {leadId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting lead: {ex.Message}");
                throw;
            }
        }

        public async Task<Lead> ConvertLeadToOpportunityAsync(Guid leadId, Opportunity opportunity)
        {
            try
            {
                var lead = await _context.Leads.FindAsync(leadId);
                if (lead == null) throw new ArgumentException("Lead not found");

                lead.Status = LeadStatus.Converted;
                lead.UpdatedAt = DateTime.UtcNow;

                opportunity.Id = Guid.NewGuid();
                opportunity.CompanyId = lead.CompanyId;
                opportunity.CreatedAt = DateTime.UtcNow;
                opportunity.Stage = OpportunityStage.Prospecting;
                opportunity.LeadId = leadId;

                _context.Opportunities.Add(opportunity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lead {leadId} converted to opportunity {opportunity.Id}");
                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error converting lead to opportunity: {ex.Message}");
                throw;
            }
        }

        public async Task<OpportunityDto?> ConvertLeadToOpportunityAsync(Guid leadId, CreateOpportunityDto createOpportunityDto)
        {
            try
            {
                var lead = await _context.Leads.FindAsync(leadId);
                if (lead == null) return null;

                // Update lead status to converted
                lead.Status = LeadStatus.Converted;
                lead.UpdatedAt = DateTime.UtcNow;

                // Create opportunity from lead
                var opportunity = new Opportunity
                {
                    Id = Guid.NewGuid(),
                    CompanyId = lead.CompanyId,
                    Name = createOpportunityDto.Name,
                    Description = createOpportunityDto.Description,
                    EstimatedValue = createOpportunityDto.Value,
                    Stage = createOpportunityDto.Stage,
                    Probability = createOpportunityDto.Probability,
                    ExpectedCloseDate = createOpportunityDto.ExpectedCloseDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Opportunities.Add(opportunity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Lead {LeadId} converted to opportunity {OpportunityId}", leadId, opportunity.Id);

                return new OpportunityDto
                {
                    Id = opportunity.Id,
                    CompanyId = opportunity.CompanyId,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    Value = opportunity.EstimatedValue,
                    Stage = opportunity.Stage,
                    Probability = opportunity.Probability,
                    ExpectedCloseDate = opportunity.ExpectedCloseDate,
                    ActualCloseDate = opportunity.ActualCloseDate,
                    Notes = opportunity.Notes,
                    AssignedUserId = opportunity.AssignedToUserId,
                    LeadId = opportunity.LeadId,
                    CreatedAt = opportunity.CreatedAt,
                    UpdatedAt = opportunity.UpdatedAt,
                    AssignedUserName = string.Empty,
                    CompanyName = string.Empty,
                    LeadName = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting lead {LeadId} to opportunity", leadId);
                throw;
            }
        }

        #endregion

        #region Opportunity Management

        public async Task<Opportunity> CreateOpportunityAsync(Opportunity opportunity)
        {
            try
            {
                opportunity.Id = Guid.NewGuid();
                opportunity.CreatedAt = DateTime.UtcNow;
                opportunity.Stage = OpportunityStage.Prospecting;

                _context.Opportunities.Add(opportunity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Opportunity created successfully with ID: {opportunity.Id}");
                return opportunity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating opportunity: {ex.Message}");
                throw;
            }
        }

        public async Task<OpportunityDto> CreateOpportunityAsync(CreateOpportunityDto createOpportunityDto)
        {
            try
            {
                var opportunity = new Opportunity
                {
                    Id = Guid.NewGuid(),
                    CompanyId = createOpportunityDto.CompanyId,
                    Name = createOpportunityDto.Name,
                    Description = createOpportunityDto.Description,
                    EstimatedValue = createOpportunityDto.Value,
                    Stage = createOpportunityDto.Stage,
                    Probability = createOpportunityDto.Probability,
                    ExpectedCloseDate = createOpportunityDto.ExpectedCloseDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Opportunities.Add(opportunity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Opportunity created successfully with ID: {opportunity.Id}");
                
                return new OpportunityDto
                {
                    Id = opportunity.Id,
                    CompanyId = opportunity.CompanyId,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    Value = opportunity.EstimatedValue,
                    Stage = opportunity.Stage,
                    Probability = opportunity.Probability,
                    ExpectedCloseDate = opportunity.ExpectedCloseDate,
                    ActualCloseDate = opportunity.ActualCloseDate,
                    Notes = opportunity.Notes,
                    AssignedUserId = opportunity.AssignedToUserId,
                    LeadId = opportunity.LeadId,
                    CreatedAt = opportunity.CreatedAt,
                    UpdatedAt = opportunity.UpdatedAt,
                    AssignedUserName = string.Empty,
                    CompanyName = string.Empty,
                    LeadName = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating opportunity from DTO: {ex.Message}");
                throw;
            }
        }

        public async Task<OpportunityDto?> GetOpportunityByIdAsync(Guid opportunityId)
        {
            try
            {
                var opportunity = await _context.Opportunities
                    .FirstOrDefaultAsync(o => o.Id == opportunityId);

                if (opportunity == null) return null;

                return new OpportunityDto
                {
                    Id = opportunity.Id,
                    CompanyId = opportunity.CompanyId,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    Value = opportunity.EstimatedValue,
                    Stage = opportunity.Stage,
                    Probability = opportunity.Probability,
                    ExpectedCloseDate = opportunity.ExpectedCloseDate,
                    ActualCloseDate = opportunity.ActualCloseDate,
                    Notes = opportunity.Notes,
                    AssignedUserId = opportunity.AssignedToUserId,
                    LeadId = opportunity.LeadId,
                    CreatedAt = opportunity.CreatedAt,
                    UpdatedAt = opportunity.UpdatedAt,
                    AssignedUserName = string.Empty,
                    CompanyName = string.Empty,
                    LeadName = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving opportunity DTO with ID: {OpportunityId}", opportunityId);
                throw;
            }
        }

        public async Task<IEnumerable<OpportunityListDto>> GetOpportunitiesAsync(Guid companyId, int page, int pageSize)
        {
            try
            {
                var opportunities = await _context.Opportunities
                    .Where(o => o.CompanyId == companyId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new OpportunityListDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Value = o.EstimatedValue,
                        Stage = o.Stage,
                        Probability = o.Probability,
                        ExpectedCloseDate = o.ExpectedCloseDate,
                        CreatedAt = o.CreatedAt
                    })
                    .ToListAsync();

                return opportunities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving opportunities for company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByCompanyAsync(Guid companyId)
        {
            return await _context.Opportunities
                .Include(o => o.AssignedUser)
                .Include(o => o.Lead)
                .Where(o => o.CompanyId == companyId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByStageAsync(Guid companyId, OpportunityStage stage)
        {
            return await _context.Opportunities
                .Include(o => o.AssignedUser)
                .Include(o => o.Lead)
                .Where(o => o.CompanyId == companyId && o.Stage == stage)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Opportunity> UpdateOpportunityAsync(Opportunity opportunity)
        {
            try
            {
                opportunity.UpdatedAt = DateTime.UtcNow;
                _context.Opportunities.Update(opportunity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Opportunity updated successfully with ID: {opportunity.Id}");
                return opportunity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating opportunity: {ex.Message}");
                throw;
            }
        }

        public async Task<OpportunityDto?> UpdateOpportunityAsync(Guid id, UpdateOpportunityDto updateOpportunityDto)
        {
            try
            {
                var opportunity = await _context.Opportunities.FindAsync(id);
                if (opportunity == null) return null;

                if (updateOpportunityDto.Name != null) opportunity.Name = updateOpportunityDto.Name;
                if (updateOpportunityDto.Description != null) opportunity.Description = updateOpportunityDto.Description;
                if (updateOpportunityDto.Value.HasValue) opportunity.EstimatedValue = updateOpportunityDto.Value.Value;
                if (updateOpportunityDto.Stage.HasValue) opportunity.Stage = updateOpportunityDto.Stage.Value;
                if (updateOpportunityDto.Probability.HasValue) opportunity.Probability = updateOpportunityDto.Probability.Value;
                if (updateOpportunityDto.ExpectedCloseDate.HasValue) opportunity.ExpectedCloseDate = updateOpportunityDto.ExpectedCloseDate;
                if (updateOpportunityDto.ActualCloseDate.HasValue) opportunity.ActualCloseDate = updateOpportunityDto.ActualCloseDate;
                if (updateOpportunityDto.Notes != null) opportunity.Notes = updateOpportunityDto.Notes;
                if (updateOpportunityDto.AssignedUserId.HasValue) opportunity.AssignedToUserId = updateOpportunityDto.AssignedUserId;
                if (updateOpportunityDto.LeadId.HasValue) opportunity.LeadId = updateOpportunityDto.LeadId;
                opportunity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Opportunity updated successfully with ID: {OpportunityId}", id);

                return new OpportunityDto
                {
                    Id = opportunity.Id,
                    CompanyId = opportunity.CompanyId,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    Value = opportunity.EstimatedValue,
                    Stage = opportunity.Stage,
                    Probability = opportunity.Probability,
                    ExpectedCloseDate = opportunity.ExpectedCloseDate,
                    ActualCloseDate = opportunity.ActualCloseDate,
                    Notes = opportunity.Notes,
                    AssignedUserId = opportunity.AssignedToUserId,
                    LeadId = opportunity.LeadId,
                    CreatedAt = opportunity.CreatedAt,
                    UpdatedAt = opportunity.UpdatedAt,
                    AssignedUserName = string.Empty,
                    CompanyName = string.Empty,
                    LeadName = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating opportunity with ID: {OpportunityId}", id);
                throw;
            }
        }

        public async Task<OpportunityDto?> UpdateOpportunityStageAsync(Guid id, OpportunityStageUpdateDto stageUpdateDto)
        {
            try
            {
                var opportunity = await _context.Opportunities.FindAsync(id);
                if (opportunity == null) return null;

                opportunity.Stage = stageUpdateDto.Stage;
                opportunity.Probability = stageUpdateDto.Probability;
                opportunity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Opportunity stage updated successfully with ID: {OpportunityId}", id);

                return new OpportunityDto
                {
                    Id = opportunity.Id,
                    CompanyId = opportunity.CompanyId,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    Value = opportunity.EstimatedValue,
                    Stage = opportunity.Stage,
                    Probability = opportunity.Probability,
                    ExpectedCloseDate = opportunity.ExpectedCloseDate,
                    ActualCloseDate = opportunity.ActualCloseDate,
                    Notes = opportunity.Notes,
                    AssignedUserId = opportunity.AssignedToUserId,
                    LeadId = opportunity.LeadId,
                    CreatedAt = opportunity.CreatedAt,
                    UpdatedAt = opportunity.UpdatedAt,
                    AssignedUserName = string.Empty,
                    CompanyName = string.Empty,
                    LeadName = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating opportunity stage with ID: {OpportunityId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteOpportunityAsync(Guid opportunityId)
        {
            try
            {
                var opportunity = await _context.Opportunities.FindAsync(opportunityId);
                if (opportunity == null) return false;

                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Opportunity deleted successfully with ID: {opportunityId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting opportunity: {ex.Message}");
                throw;
            }
        }

        public async Task<Opportunity> CloseOpportunityAsync(Guid opportunityId, bool isWon, decimal? actualValue = null)
        {
            try
            {
                var opportunity = await _context.Opportunities.FindAsync(opportunityId);
                if (opportunity == null) throw new ArgumentException("Opportunity not found");

                opportunity.Stage = isWon ? OpportunityStage.ClosedWon : OpportunityStage.ClosedLost;
                opportunity.CloseDate = DateTime.UtcNow;
                opportunity.UpdatedAt = DateTime.UtcNow;
                
                if (actualValue.HasValue)
                    opportunity.ActualValue = actualValue.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Opportunity {opportunityId} closed as {(isWon ? "Won" : "Lost")}");
                return opportunity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error closing opportunity: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Campaign Management

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            try
            {
                campaign.Id = Guid.NewGuid();
                campaign.CreatedAt = DateTime.UtcNow;
                campaign.Status = CampaignStatus.Draft;

                _context.Campaigns.Add(campaign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Campaign created successfully with ID: {campaign.Id}");
                return campaign;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating campaign: {ex.Message}");
                throw;
            }
        }

        public async Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto createCampaignDto)
        {
            try
            {
                var campaign = new Campaign
                {
                    Id = Guid.NewGuid(),
                    CompanyId = createCampaignDto.CompanyId,
                    Name = createCampaignDto.Name,
                    Description = createCampaignDto.Description,
                    Type = createCampaignDto.Type,
                    Status = createCampaignDto.Status,
                    Budget = createCampaignDto.Budget,
                    StartDate = createCampaignDto.StartDate,
                    EndDate = createCampaignDto.EndDate,
                    TargetAudience = createCampaignDto.TargetAudience,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Campaigns.Add(campaign);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Campaign created successfully with ID: {CampaignId}", campaign.Id);

                return new CampaignDto
                {
                    Id = campaign.Id,
                    CompanyId = campaign.CompanyId,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    Type = campaign.Type,
                    Status = campaign.Status,
                    Budget = campaign.Budget,
                    ActualCost = campaign.ActualCost,
                    StartDate = campaign.StartDate,
                    EndDate = campaign.EndDate,
                    TargetAudience = campaign.TargetAudience,
                    ActualReach = campaign.ActualReach,
                    ConversionRate = campaign.ConversionRate,
                    ROI = campaign.ROI,
                    Channel = campaign.Channel,
                    Notes = campaign.Notes,
                    AssignedUserId = campaign.AssignedToUserId,
                    CreatedAt = campaign.CreatedAt,
                    UpdatedAt = campaign.UpdatedAt,
                    CompanyName = string.Empty, // Will be populated if needed
                    LeadsCount = 0 // Will be calculated if needed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<CampaignDto?> GetCampaignByIdAsync(Guid campaignId)
        {
            try
            {
                var campaign = await _context.Campaigns.FindAsync(campaignId);
                if (campaign == null) return null;

                return new CampaignDto
                {
                    Id = campaign.Id,
                    CompanyId = campaign.CompanyId,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    Type = campaign.Type,
                    Status = campaign.Status,
                    Budget = campaign.Budget,
                    ActualCost = campaign.ActualCost,
                    StartDate = campaign.StartDate,
                    EndDate = campaign.EndDate,
                    TargetAudience = campaign.TargetAudience,
                    ActualReach = campaign.ActualReach,
                    ConversionRate = campaign.ConversionRate,
                    ROI = campaign.ROI,
                    Channel = campaign.Channel,
                    Notes = campaign.Notes,
                    AssignedUserId = campaign.AssignedToUserId,
                    CreatedAt = campaign.CreatedAt,
                    UpdatedAt = campaign.UpdatedAt,
                    CompanyName = string.Empty,
                    LeadsCount = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign with ID: {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<IEnumerable<CampaignListDto>> GetCampaignsAsync(Guid companyId, int page = 1, int pageSize = 10)
        {
            try
            {
                var campaigns = await _context.Campaigns
                    .Where(c => c.CompanyId == companyId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CampaignListDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = c.Type,
                        Status = c.Status,
                        Budget = c.Budget,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} campaigns for company {CompanyId}", campaigns.Count(), companyId);
                return campaigns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaigns for company: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<Campaign>> GetCampaignsByCompanyAsync(Guid companyId)
        {
            return await _context.Campaigns
                .Where(c => c.CompanyId == companyId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Campaign> UpdateCampaignAsync(Campaign campaign)
        {
            try
            {
                campaign.UpdatedAt = DateTime.UtcNow;
                _context.Campaigns.Update(campaign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Campaign updated successfully with ID: {campaign.Id}");
                return campaign;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating campaign: {ex.Message}");
                throw;
            }
        }

        public async Task<CampaignDto?> UpdateCampaignAsync(Guid id, UpdateCampaignDto updateCampaignDto)
        {
            try
            {
                var campaign = await _context.Campaigns.FindAsync(id);
                if (campaign == null) return null;

                campaign.Name = updateCampaignDto.Name;
                campaign.Description = updateCampaignDto.Description;
                if (updateCampaignDto.Type.HasValue) campaign.Type = updateCampaignDto.Type.Value;
                if (updateCampaignDto.Status.HasValue) campaign.Status = updateCampaignDto.Status.Value;
                if (updateCampaignDto.Budget.HasValue) campaign.Budget = updateCampaignDto.Budget.Value;
                if (updateCampaignDto.StartDate.HasValue) campaign.StartDate = updateCampaignDto.StartDate.Value;
                campaign.EndDate = updateCampaignDto.EndDate;
                if (updateCampaignDto.TargetAudience.HasValue) campaign.TargetAudience = updateCampaignDto.TargetAudience.Value;
                campaign.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Campaign updated successfully with ID: {CampaignId}", id);

                return new CampaignDto
                {
                    Id = campaign.Id,
                    CompanyId = campaign.CompanyId,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    Type = campaign.Type,
                    Status = campaign.Status,
                    Budget = campaign.Budget,
                    ActualCost = campaign.ActualCost,
                    StartDate = campaign.StartDate,
                    EndDate = campaign.EndDate,
                    TargetAudience = campaign.TargetAudience,
                    ActualReach = campaign.ActualReach,
                    ConversionRate = campaign.ConversionRate,
                    ROI = campaign.ROI,
                    Channel = campaign.Channel,
                    Notes = campaign.Notes,
                    AssignedUserId = campaign.AssignedToUserId,
                    CreatedAt = campaign.CreatedAt,
                    UpdatedAt = campaign.UpdatedAt,
                    CompanyName = string.Empty,
                    LeadsCount = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating campaign with ID: {CampaignId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCampaignAsync(Guid campaignId)
        {
            try
            {
                var campaign = await _context.Campaigns.FindAsync(campaignId);
                if (campaign == null) return false;

                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Campaign deleted successfully with ID: {campaignId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting campaign: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddLeadToCampaignAsync(Guid campaignId, Guid leadId)
        {
            try
            {
                // Check if the relationship already exists
                var existingCampaignLead = await _context.CampaignLeads
                    .FirstOrDefaultAsync(cl => cl.CampaignId == campaignId && cl.LeadId == leadId);
                
                if (existingCampaignLead != null) return false;

                var campaignLead = new CampaignLead
                {
                    CampaignId = campaignId,
                    LeadId = leadId,
                    AddedDate = DateTime.UtcNow
                };

                _context.CampaignLeads.Add(campaignLead);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lead {LeadId} added to campaign {CampaignId}", leadId, campaignId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding lead to campaign: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> RemoveLeadFromCampaignAsync(Guid campaignId, Guid leadId)
        {
            try
            {
                var campaignLead = await _context.CampaignLeads
                    .FirstOrDefaultAsync(cl => cl.CampaignId == campaignId && cl.LeadId == leadId);
                
                if (campaignLead == null) return false;

                _context.CampaignLeads.Remove(campaignLead);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lead {leadId} removed from campaign {campaignId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing lead from campaign: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<CampaignLead>> GetCampaignLeadsAsync(Guid campaignId)
        {
            return await _context.CampaignLeads
                .Include(cl => cl.Lead)
                .Include(cl => cl.Campaign)
                .Where(cl => cl.CampaignId == campaignId)
                .ToListAsync();
        }

        #endregion

        #region Customer Tracking

        public async Task<CustomerTracking> CreateCustomerTrackingAsync(CustomerTracking tracking)
        {
            try
            {
                tracking.Id = Guid.NewGuid();
                tracking.InteractionDate = DateTime.UtcNow;

                _context.CustomerTrackings.Add(tracking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Customer tracking created successfully with ID: {tracking.Id}");
                return tracking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating customer tracking: {ex.Message}");
                throw;
            }
        }

        public async Task<CustomerTracking> GetCustomerTrackingByIdAsync(Guid trackingId)
        {
            return await _context.CustomerTrackings
                .Include(ct => ct.Company)
                .Include(ct => ct.Customer)
                .Include(ct => ct.User)
                .FirstOrDefaultAsync(ct => ct.Id == trackingId);
        }

        public async Task<IEnumerable<CustomerTracking>> GetCustomerTrackingsByCustomerAsync(Guid customerId)
        {
            return await _context.CustomerTrackings
                .Include(ct => ct.User)
                .Where(ct => ct.CustomerId == customerId)
                .OrderByDescending(ct => ct.InteractionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerTracking>> GetCustomerTrackingsByCompanyAsync(Guid companyId)
        {
            return await _context.CustomerTrackings
                .Include(ct => ct.Customer)
                .Include(ct => ct.User)
                .Where(ct => ct.CompanyId == companyId)
                .OrderByDescending(ct => ct.InteractionDate)
                .ToListAsync();
        }

        public async Task<CustomerTracking> UpdateCustomerTrackingAsync(CustomerTracking tracking)
        {
            try
            {
                _context.CustomerTrackings.Update(tracking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Customer tracking updated successfully with ID: {tracking.Id}");
                return tracking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating customer tracking: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteCustomerTrackingAsync(Guid trackingId)
        {
            try
            {
                var tracking = await _context.CustomerTrackings.FindAsync(trackingId);
                if (tracking == null) return false;

                _context.CustomerTrackings.Remove(tracking);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Customer tracking deleted successfully with ID: {trackingId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer tracking: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Analytics and Reports

        public async Task<int> GetLeadCountByStatusAsync(Guid companyId, LeadStatus status)
        {
            return await _context.Leads
                .CountAsync(l => l.CompanyId == companyId && l.Status == status);
        }

        public async Task<decimal> GetOpportunityValueByStageAsync(Guid companyId, OpportunityStage stage)
        {
            return await _context.Opportunities
                .Where(o => o.CompanyId == companyId && o.Stage == stage)
                .SumAsync(o => o.EstimatedValue);
        }

        public async Task<decimal> GetCampaignROIAsync(Guid campaignId)
        {
            var campaign = await _context.Campaigns.FindAsync(campaignId);
            if (campaign == null || campaign.Budget == 0) return 0;

            var conversions = await _context.CampaignLeads
                .Where(cl => cl.CampaignId == campaignId && cl.IsConverted)
                .CountAsync();

            var revenue = await _context.Opportunities
                .Where(o => o.Lead != null && 
                           o.Lead.CampaignLeads.Any(cl => cl.CampaignId == campaignId) &&
                           o.Stage == OpportunityStage.ClosedWon)
                .SumAsync(o => o.ActualValue);

            return campaign.Budget > 0 ? (revenue - campaign.Budget) / campaign.Budget * 100 : 0;
        }

        public async Task<IEnumerable<Lead>> GetLeadsBySourceAsync(Guid companyId, LeadSource source)
        {
            return await _context.Leads
                .Include(l => l.AssignedUser)
                .Where(l => l.CompanyId == companyId && l.Source == source)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerTracking>> GetCustomerInteractionsByTypeAsync(Guid companyId, CustomerInteractionType type)
        {
            return await _context.CustomerTrackings
                .Include(ct => ct.Customer)
                .Include(ct => ct.User)
                .Where(ct => ct.CompanyId == companyId && ct.InteractionType == type)
                .OrderByDescending(ct => ct.InteractionDate)
                .ToListAsync();
        }

        public async Task<CampaignStatsDto?> GetCampaignStatsAsync(Guid campaignId)
        {
            try
            {
                var campaign = await _context.Campaigns.FindAsync(campaignId);
                if (campaign == null) return null;

                var totalLeads = await _context.CampaignLeads
                    .CountAsync(cl => cl.CampaignId == campaignId);

                var convertedLeads = await _context.CampaignLeads
                    .Where(cl => cl.CampaignId == campaignId)
                    .Join(_context.Opportunities, cl => cl.LeadId, o => o.LeadId, (cl, o) => o)
                    .CountAsync();

                var conversionRate = totalLeads > 0 ? (decimal)convertedLeads / totalLeads * 100 : 0;

                var totalRevenue = await _context.CampaignLeads
                    .Where(cl => cl.CampaignId == campaignId)
                    .Join(_context.Opportunities, cl => cl.LeadId, o => o.LeadId, (cl, o) => o)
                    .Where(o => o.Stage == OpportunityStage.ClosedWon)
                    .SumAsync(o => o.EstimatedValue);

                var roi = campaign.Budget > 0 
                    ? (totalRevenue - campaign.Budget) / campaign.Budget * 100 
                    : 0m;

                var costPerLead = totalLeads > 0 && campaign.Budget > 0 
                    ? campaign.Budget / totalLeads 
                    : 0m;

                return new CampaignStatsDto
                {
                    CampaignId = campaignId,
                    CampaignName = campaign.Name,
                    TotalLeads = totalLeads,
                    QualifiedLeads = convertedLeads, // Usando convertedLeads como qualified leads
                    ConvertedLeads = convertedLeads,
                    ConversionRate = Math.Round(conversionRate, 2),
                    ROI = Math.Round(roi, 2),
                    CostPerLead = Math.Round(costPerLead, 2),
                    Revenue = totalRevenue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign stats for campaign {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<object> GetCRMDashboardAsync(Guid companyId)
        {
            try
            {
                var totalLeads = await _context.Leads.CountAsync(l => l.CompanyId == companyId);
                var totalOpportunities = await _context.Opportunities.CountAsync(o => o.CompanyId == companyId);
                var totalCampaigns = await _context.Campaigns.CountAsync(c => c.CompanyId == companyId);

                var leadsThisMonth = await _context.Leads
                    .CountAsync(l => l.CompanyId == companyId && 
                               l.CreatedAt >= DateTime.UtcNow.AddDays(-30));

                var opportunitiesValue = await _context.Opportunities
                    .Where(o => o.CompanyId == companyId && o.Stage != OpportunityStage.ClosedLost)
                    .SumAsync(o => o.EstimatedValue);

                var wonOpportunitiesValue = await _context.Opportunities
                    .Where(o => o.CompanyId == companyId && o.Stage == OpportunityStage.ClosedWon)
                    .SumAsync(o => o.EstimatedValue);

                var conversionRate = totalLeads > 0 
                    ? (decimal)await _context.Opportunities.CountAsync(o => o.CompanyId == companyId) / totalLeads * 100 
                    : 0;

                var recentLeads = await _context.Leads
                    .Where(l => l.CompanyId == companyId)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(5)
                    .Select(l => new { l.Id, l.FirstName, l.LastName, l.Email, l.Status, l.CreatedAt })
                    .ToListAsync();

                return new
                {
                    CompanyId = companyId,
                    TotalLeads = totalLeads,
                    TotalOpportunities = totalOpportunities,
                    TotalCampaigns = totalCampaigns,
                    LeadsThisMonth = leadsThisMonth,
                    OpportunitiesValue = opportunitiesValue,
                    WonOpportunitiesValue = wonOpportunitiesValue,
                    ConversionRate = Math.Round(conversionRate, 2),
                    RecentLeads = recentLeads
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CRM dashboard for company {CompanyId}", companyId);
                throw;
            }
        }

        #endregion
    }
}