using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para convertir un lead en una oportunidad
    /// </summary>
    public class ConvertLeadCommandHandler : IRequestHandler<ConvertLeadCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConvertLeadCommandHandler> _logger;

        public ConvertLeadCommandHandler(
            ApplicationDbContext context,
            ILogger<ConvertLeadCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(ConvertLeadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Converting lead {LeadId} to opportunity for company {CompanyId}", 
                    request.LeadId, request.CompanyId);

                var lead = await _context.Leads
                    .FirstOrDefaultAsync(l => l.Id == request.LeadId && l.CompanyId == request.CompanyId, cancellationToken);

                if (lead == null)
                {
                    throw new InvalidOperationException($"Lead with ID {request.LeadId} not found in company {request.CompanyId}.");
                }

                // Verificar que el lead no esté ya convertido
                if (lead.Status == LeadStatus.Converted)
                {
                    throw new InvalidOperationException($"Lead {request.LeadId} is already converted.");
                }

                // Crear la oportunidad
                var opportunity = new Opportunity
                {
                    Id = Guid.NewGuid(),
                    CompanyId = request.CompanyId,
                    LeadId = request.LeadId,
                    Name = request.OpportunityName,
                    Description = request.OpportunityDescription,
                    EstimatedValue = request.EstimatedValue ?? 0,
                    ExpectedCloseDate = request.ExpectedCloseDate,
                    Probability = (int)(request.Probability ?? 0),
                    AssignedToUserId = request.AssignedToUserId,
                    Notes = request.Notes,
                    Stage = OpportunityStage.Prospecting,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId.ToString(),
                    IsActive = true
                };

                // Actualizar el lead a convertido
                lead.Status = LeadStatus.Converted;
                lead.UpdatedAt = DateTime.UtcNow;
                lead.ModifiedBy = request.UserId.ToString();

                // Si se solicita crear cliente, crear un Customer
                if (request.CreateCustomer)
                {
                    var customer = new CompanyService.Core.Entities.Customer
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = request.CompanyId,
                        Name = $"{lead.FirstName} {lead.LastName}".Trim(),
                        Email = lead.Email,
                        Phone = lead.Phone,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.Customers.Add(customer);
                    opportunity.CustomerId = customer.Id;
                }

                // Agregar la oportunidad
                _context.Opportunities.Add(opportunity);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully converted lead {LeadId} to opportunity {OpportunityId}", 
                    request.LeadId, opportunity.Id);

                return opportunity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting lead {LeadId} for company {CompanyId}", 
                    request.LeadId, request.CompanyId);
                throw;
            }
        }
    }
}
