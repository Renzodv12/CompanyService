using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para eliminar una oportunidad
    /// </summary>
    public class DeleteOpportunityCommandHandler : IRequestHandler<DeleteOpportunityCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteOpportunityCommandHandler> _logger;

        public DeleteOpportunityCommandHandler(
            ApplicationDbContext context,
            ILogger<DeleteOpportunityCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting opportunity {OpportunityId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var opportunity = await _context.Opportunities
                    .FirstOrDefaultAsync(o => o.Id == request.Id && o.CompanyId == request.CompanyId, cancellationToken);

                if (opportunity == null)
                {
                    throw new InvalidOperationException($"Opportunity with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Verificar si la oportunidad está cerrada (ganada o perdida)
                if (opportunity.IsWon || opportunity.IsLost)
                {
                    throw new InvalidOperationException($"Cannot delete opportunity {request.Id} because it is already closed (won or lost).");
                }

                // Eliminar la oportunidad
                _context.Opportunities.Remove(opportunity);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted opportunity {OpportunityId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting opportunity {OpportunityId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}


