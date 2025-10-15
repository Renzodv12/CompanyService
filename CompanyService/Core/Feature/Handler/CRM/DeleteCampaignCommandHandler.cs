using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para eliminar una campaña
    /// </summary>
    public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteCampaignCommandHandler> _logger;

        public DeleteCampaignCommandHandler(
            ApplicationDbContext context,
            ILogger<DeleteCampaignCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting campaign {CampaignId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var campaign = await _context.Campaigns
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, cancellationToken);

                if (campaign == null)
                {
                    throw new InvalidOperationException($"Campaign with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Verificar si la campaña está activa
                if (campaign.Status == CompanyService.Core.Enums.CampaignStatus.Active)
                {
                    throw new InvalidOperationException($"Cannot delete campaign {request.Id} because it is currently active. Please pause or complete the campaign first.");
                }

                // Verificar si la campaña tiene leads asociados
                var hasLeads = await _context.CampaignLeads
                    .AnyAsync(cl => cl.CampaignId == request.Id, cancellationToken);

                if (hasLeads)
                {
                    _logger.LogWarning("Campaign {CampaignId} has associated leads. Removing associations first.", request.Id);
                    
                    // Eliminar asociaciones con leads
                    var campaignLeads = await _context.CampaignLeads
                        .Where(cl => cl.CampaignId == request.Id)
                        .ToListAsync(cancellationToken);
                    
                    _context.CampaignLeads.RemoveRange(campaignLeads);
                }

                // Eliminar la campaña
                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted campaign {CampaignId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting campaign {CampaignId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}


