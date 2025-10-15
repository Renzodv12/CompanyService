using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para actualizar una campaña existente
    /// </summary>
    public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateCampaignCommandHandler> _logger;

        public UpdateCampaignCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateCampaignCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating campaign {CampaignId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var campaign = await _context.Campaigns
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, cancellationToken);

                if (campaign == null)
                {
                    throw new InvalidOperationException($"Campaign with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Actualizar campos si se proporcionan
                if (!string.IsNullOrEmpty(request.Name))
                    campaign.Name = request.Name;

                if (request.Description != null)
                    campaign.Description = request.Description;

                if (!string.IsNullOrEmpty(request.Type))
                {
                    if (Enum.TryParse<CampaignType>(request.Type, true, out var type))
                        campaign.Type = type;
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    if (Enum.TryParse<CampaignStatus>(request.Status, true, out var status))
                        campaign.Status = status;
                }

                if (request.StartDate != default)
                    campaign.StartDate = request.StartDate;

                if (request.EndDate.HasValue)
                    campaign.EndDate = request.EndDate;

                if (request.Budget.HasValue)
                    campaign.Budget = request.Budget.Value;

                if (!string.IsNullOrEmpty(request.Type))
                {
                    if (Enum.TryParse<CampaignType>(request.Type, true, out var type))
                        campaign.Type = type;
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    if (Enum.TryParse<CampaignStatus>(request.Status, true, out var status))
                        campaign.Status = status;
                }

                // Actualizar timestamps
                campaign.UpdatedAt = DateTime.UtcNow;
                campaign.ModifiedBy = request.UserId.ToString();

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated campaign {CampaignId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating campaign {CampaignId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
