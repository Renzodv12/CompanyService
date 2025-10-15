using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para actualizar una oportunidad existente
    /// </summary>
    public class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateOpportunityCommandHandler> _logger;

        public UpdateOpportunityCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateOpportunityCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating opportunity {OpportunityId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var opportunity = await _context.Opportunities
                    .FirstOrDefaultAsync(o => o.Id == request.Id && o.CompanyId == request.CompanyId, cancellationToken);

                if (opportunity == null)
                {
                    throw new InvalidOperationException($"Opportunity with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Validar que el lead existe si se está cambiando
                if (request.LeadId.HasValue && request.LeadId.Value != opportunity.LeadId)
                {
                    var leadExists = await _context.Leads
                        .AnyAsync(l => l.Id == request.LeadId.Value && l.CompanyId == request.CompanyId, cancellationToken);

                    if (!leadExists)
                    {
                        throw new InvalidOperationException($"Lead with ID {request.LeadId.Value} not found in company {request.CompanyId}.");
                    }
                }

                // Actualizar campos si se proporcionan
                if (!string.IsNullOrEmpty(request.Name))
                    opportunity.Name = request.Name;

                if (request.Description != null)
                    opportunity.Description = request.Description;

                if (request.EstimatedValue.HasValue)
                    opportunity.EstimatedValue = request.EstimatedValue.Value;

                if (!string.IsNullOrEmpty(request.Stage))
                {
                    if (Enum.TryParse<OpportunityStage>(request.Stage, true, out var stage))
                        opportunity.Stage = stage;
                }

                if (request.Probability.HasValue)
                    opportunity.Probability = (int)request.Probability.Value;

                if (request.ExpectedCloseDate.HasValue)
                    opportunity.ExpectedCloseDate = request.ExpectedCloseDate;

                if (request.LeadId.HasValue)
                    opportunity.LeadId = request.LeadId;

                // Actualizar timestamps
                opportunity.UpdatedAt = DateTime.UtcNow;
                opportunity.ModifiedBy = request.UserId.ToString();

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated opportunity {OpportunityId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating opportunity {OpportunityId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
