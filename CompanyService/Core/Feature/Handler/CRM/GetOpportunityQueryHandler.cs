using MediatR;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.DTOs.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para obtener una oportunidad específica
    /// </summary>
    public class GetOpportunityQueryHandler : IRequestHandler<GetOpportunityQuery, OpportunityDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetOpportunityQueryHandler> _logger;

        public GetOpportunityQueryHandler(
            ApplicationDbContext context,
            ILogger<GetOpportunityQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OpportunityDto> Handle(GetOpportunityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting opportunity {OpportunityId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var opportunity = await _context.Opportunities
                    .Include(o => o.AssignedUser)
                    .Include(o => o.Lead)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Id == request.Id && o.CompanyId == request.CompanyId, cancellationToken);

                if (opportunity == null)
                {
                    throw new InvalidOperationException($"Opportunity with ID {request.Id} not found in company {request.CompanyId}.");
                }

                var opportunityDto = new OpportunityDto
                {
                    Id = opportunity.Id,
                    Name = opportunity.Name,
                    Description = opportunity.Description,
                    Stage = opportunity.Stage,
                    Value = opportunity.EstimatedValue,
                    Probability = opportunity.Probability,
                    ExpectedCloseDate = opportunity.ExpectedCloseDate,
                    ActualCloseDate = opportunity.ActualCloseDate,
                    Notes = opportunity.Notes,
                    CompanyId = opportunity.CompanyId,
                    LeadId = opportunity.LeadId,
                    AssignedUserId = opportunity.AssignedToUserId,
                    AssignedUserName = opportunity.AssignedUser != null 
                        ? $"{opportunity.AssignedUser.FirstName} {opportunity.AssignedUser.LastName}".Trim()
                        : null,
                    LeadName = opportunity.Lead != null 
                        ? $"{opportunity.Lead.FirstName} {opportunity.Lead.LastName}".Trim()
                        : null,
                    CreatedAt = opportunity.CreatedAt,
                    UpdatedAt = opportunity.UpdatedAt
                };

                _logger.LogInformation("Successfully retrieved opportunity {OpportunityId}", request.Id);
                return opportunityDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting opportunity {OpportunityId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
