using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para crear una nueva campaña
    /// </summary>
    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateCampaignCommandHandler> _logger;

        public CreateCampaignCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateCampaignCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating campaign for company {CompanyId} with name {Name}", 
                    request.CompanyId, request.Name);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Convertir strings a enums
                var campaignType = Enum.TryParse<CampaignType>(request.Type, true, out var type) 
                    ? type 
                    : CampaignType.Email;

                var campaignStatus = Enum.TryParse<CampaignStatus>(request.Status, true, out var status) 
                    ? status 
                    : CampaignStatus.Draft;

                // Crear la entidad Campaign
                var campaign = new Campaign
                {
                    Id = Guid.NewGuid(),
                    CompanyId = request.CompanyId,
                    Name = request.Name,
                    Description = request.Description,
                    Type = campaignType,
                    Status = campaignStatus,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Budget = request.Budget ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId.ToString(),
                    IsActive = true
                };

                // Agregar al contexto
                _context.Campaigns.Add(campaign);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created campaign with ID {CampaignId} for company {CompanyId}", 
                    campaign.Id, request.CompanyId);

                return campaign.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}
