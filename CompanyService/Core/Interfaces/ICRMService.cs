using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.DTOs.CRM;

namespace CompanyService.Core.Interfaces
{
    public interface ICRMService
    {
        // Lead Management
        Task<Lead> CreateLeadAsync(Lead lead);
        Task<LeadDto> CreateLeadAsync(CreateLeadDto createLeadDto);
        Task<LeadDto?> GetLeadByIdAsync(Guid leadId);
        Task<IEnumerable<Lead>> GetLeadsByCompanyAsync(Guid companyId);
        Task<IEnumerable<LeadListDto>> GetLeadsAsync(Guid companyId, int page, int pageSize);
        Task<IEnumerable<Lead>> GetLeadsByStatusAsync(Guid companyId, LeadStatus status);
        Task<Lead> UpdateLeadAsync(Lead lead);
        Task<LeadDto?> UpdateLeadAsync(Guid id, UpdateLeadDto updateLeadDto);
        Task<bool> DeleteLeadAsync(Guid leadId);
        Task<Lead> ConvertLeadToOpportunityAsync(Guid leadId, Opportunity opportunity);
        Task<OpportunityDto?> ConvertLeadToOpportunityAsync(Guid leadId, CreateOpportunityDto createOpportunityDto);
        
        // Opportunity Management
        Task<Opportunity> CreateOpportunityAsync(Opportunity opportunity);
        Task<OpportunityDto> CreateOpportunityAsync(CreateOpportunityDto createOpportunityDto);
        Task<OpportunityDto?> GetOpportunityByIdAsync(Guid opportunityId);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByCompanyAsync(Guid companyId);
        Task<IEnumerable<OpportunityListDto>> GetOpportunitiesAsync(Guid companyId, int page, int pageSize);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByStageAsync(Guid companyId, OpportunityStage stage);
        Task<Opportunity> UpdateOpportunityAsync(Opportunity opportunity);
        Task<OpportunityDto?> UpdateOpportunityAsync(Guid id, UpdateOpportunityDto updateOpportunityDto);
        Task<OpportunityDto?> UpdateOpportunityStageAsync(Guid id, OpportunityStageUpdateDto stageUpdateDto);
        Task<bool> DeleteOpportunityAsync(Guid opportunityId);
        Task<Opportunity> CloseOpportunityAsync(Guid opportunityId, bool isWon, decimal? actualValue = null);
        
        // Campaign Management
        Task<Campaign> CreateCampaignAsync(Campaign campaign);
        Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto createCampaignDto);
        Task<CampaignDto?> GetCampaignByIdAsync(Guid campaignId);
        Task<IEnumerable<Campaign>> GetCampaignsByCompanyAsync(Guid companyId);
        Task<IEnumerable<CampaignListDto>> GetCampaignsAsync(Guid companyId, int page, int pageSize);
        Task<Campaign> UpdateCampaignAsync(Campaign campaign);
        Task<CampaignDto?> UpdateCampaignAsync(Guid id, UpdateCampaignDto updateCampaignDto);
        Task<bool> DeleteCampaignAsync(Guid campaignId);
        Task<bool> AddLeadToCampaignAsync(Guid campaignId, Guid leadId);
        Task<bool> RemoveLeadFromCampaignAsync(Guid campaignId, Guid leadId);
        Task<IEnumerable<CampaignLead>> GetCampaignLeadsAsync(Guid campaignId);
        Task<CampaignStatsDto?> GetCampaignStatsAsync(Guid campaignId);
        Task<object> GetCRMDashboardAsync(Guid companyId);
        
        // Customer Tracking
        Task<CustomerTracking> CreateCustomerTrackingAsync(CustomerTracking tracking);
        Task<CustomerTracking> GetCustomerTrackingByIdAsync(Guid trackingId);
        Task<IEnumerable<CustomerTracking>> GetCustomerTrackingsByCustomerAsync(Guid customerId);
        Task<IEnumerable<CustomerTracking>> GetCustomerTrackingsByCompanyAsync(Guid companyId);
        Task<CustomerTracking> UpdateCustomerTrackingAsync(CustomerTracking tracking);
        Task<bool> DeleteCustomerTrackingAsync(Guid trackingId);
        
        // Analytics and Reports
        Task<int> GetLeadCountByStatusAsync(Guid companyId, LeadStatus status);
        Task<decimal> GetOpportunityValueByStageAsync(Guid companyId, OpportunityStage stage);
        Task<decimal> GetCampaignROIAsync(Guid campaignId);
        Task<IEnumerable<Lead>> GetLeadsBySourceAsync(Guid companyId, LeadSource source);
        Task<IEnumerable<CustomerTracking>> GetCustomerInteractionsByTypeAsync(Guid companyId, CustomerInteractionType type);
    }
}