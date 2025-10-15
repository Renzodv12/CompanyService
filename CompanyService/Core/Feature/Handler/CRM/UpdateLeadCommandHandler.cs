using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para actualizar un lead existente
    /// </summary>
    public class UpdateLeadCommandHandler : IRequestHandler<UpdateLeadCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateLeadCommandHandler> _logger;

        public UpdateLeadCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateLeadCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating lead {LeadId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var lead = await _context.Leads
                    .FirstOrDefaultAsync(l => l.Id == request.Id && l.CompanyId == request.CompanyId, cancellationToken);

                if (lead == null)
                {
                    throw new InvalidOperationException($"Lead with ID {request.Id} not found in company {request.CompanyId}.");
                }

                // Validar email único si se está cambiando
                if (!string.IsNullOrEmpty(request.Email) && request.Email != lead.Email)
                {
                    var emailExists = await _context.Leads
                        .AnyAsync(l => l.CompanyId == request.CompanyId && 
                                 l.Email == request.Email && 
                                 l.Id != request.Id, cancellationToken);

                    if (emailExists)
                    {
                        throw new InvalidOperationException($"A lead with email {request.Email} already exists in this company.");
                    }
                }

                // Actualizar campos si se proporcionan
                if (!string.IsNullOrEmpty(request.FirstName))
                    lead.FirstName = request.FirstName;

                if (!string.IsNullOrEmpty(request.LastName))
                    lead.LastName = request.LastName;

                if (!string.IsNullOrEmpty(request.Email))
                    lead.Email = request.Email;

                if (request.Phone != null)
                    lead.Phone = request.Phone;

                if (request.Company != null)
                    lead.CompanyName = request.Company;

                if (request.Position != null)
                    lead.JobTitle = request.Position;

                if (!string.IsNullOrEmpty(request.Source))
                {
                    if (Enum.TryParse<LeadSource>(request.Source, true, out var source))
                        lead.Source = source;
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    if (Enum.TryParse<LeadStatus>(request.Status, true, out var status))
                        lead.Status = status;
                }

                if (request.Notes != null)
                    lead.Notes = request.Notes;

                // Actualizar timestamps
                lead.UpdatedAt = DateTime.UtcNow;
                lead.ModifiedBy = request.UserId.ToString();

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated lead {LeadId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lead {LeadId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
