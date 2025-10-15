using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para crear una nueva oportunidad
    /// </summary>
    public class CreateOpportunityCommandHandler : IRequestHandler<CreateOpportunityCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateOpportunityCommandHandler> _logger;

        public CreateOpportunityCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateOpportunityCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating opportunity for company {CompanyId} with name {Name}", 
                    request.CompanyId, request.Name);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Validar que el lead existe si se proporciona
                if (request.LeadId.HasValue)
                {
                    var leadExists = await _context.Leads
                        .AnyAsync(l => l.Id == request.LeadId.Value && l.CompanyId == request.CompanyId, cancellationToken);

                    if (!leadExists)
                    {
                        throw new InvalidOperationException($"Lead with ID {request.LeadId.Value} not found in company {request.CompanyId}.");
                    }
                }

                // Convertir string a enum
                var stage = Enum.TryParse<OpportunityStage>(request.Stage, true, out var opportunityStage) 
                    ? opportunityStage 
                    : OpportunityStage.Prospecting;

                // Crear la entidad Opportunity
                var opportunity = new Opportunity
                {
                    Id = Guid.NewGuid(),
                    CompanyId = request.CompanyId,
                    Name = request.Name,
                    Description = request.Description,
                    LeadId = request.LeadId,
                    Stage = stage,
                    EstimatedValue = request.EstimatedValue ?? 0,
                    Probability = (int)(request.Probability ?? 0),
                    ExpectedCloseDate = request.ExpectedCloseDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId.ToString(),
                    IsActive = true
                };

                // Agregar al contexto
                _context.Opportunities.Add(opportunity);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created opportunity with ID {OpportunityId} for company {CompanyId}", 
                    opportunity.Id, request.CompanyId);

                return opportunity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating opportunity for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}
