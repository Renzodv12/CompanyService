using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para eliminar un lead
    /// </summary>
    public class DeleteLeadCommandHandler : IRequestHandler<DeleteLeadCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteLeadCommandHandler> _logger;

        public DeleteLeadCommandHandler(
            ApplicationDbContext context,
            ILogger<DeleteLeadCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteLeadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting lead {LeadId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var lead = await _context.Leads
                    .FirstOrDefaultAsync(l => l.Id == request.Id && l.CompanyId == request.CompanyId, cancellationToken);

                if (lead == null)
                {
                    throw new InvalidOperationException($"Lead with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Verificar si el lead tiene oportunidades asociadas
                var hasOpportunities = await _context.Opportunities
                    .AnyAsync(o => o.LeadId == request.Id, cancellationToken);

                if (hasOpportunities)
                {
                    throw new InvalidOperationException($"Cannot delete lead {request.Id} because it has associated opportunities. Convert or remove opportunities first.");
                }

                // Eliminar el lead
                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted lead {LeadId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lead {LeadId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}


