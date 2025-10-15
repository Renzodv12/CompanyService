using MediatR;
using CompanyService.Core.Feature.Commands.CRM;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para crear un nuevo lead
    /// </summary>
    public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateLeadCommandHandler> _logger;

        public CreateLeadCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateLeadCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating lead for company {CompanyId} with email {Email}", 
                    request.CompanyId, request.Email);

                // Validar que la compañía existe
                var companyExists = await _context.Companies
                    .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

                if (!companyExists)
                {
                    throw new InvalidOperationException($"Company with ID {request.CompanyId} does not exist.");
                }

                // Validar que el email no esté duplicado en la compañía
                var emailExists = await _context.Leads
                    .AnyAsync(l => l.CompanyId == request.CompanyId && l.Email == request.Email, cancellationToken);

                if (emailExists)
                {
                    throw new InvalidOperationException($"A lead with email {request.Email} already exists in this company.");
                }

                // Convertir strings a enums
                var leadSource = Enum.TryParse<LeadSource>(request.Source, true, out var source) 
                    ? source 
                    : LeadSource.Website;

                var leadStatus = Enum.TryParse<LeadStatus>(request.Status, true, out var status) 
                    ? status 
                    : LeadStatus.New;

                // Crear la entidad Lead
                var lead = new Lead
                {
                    Id = Guid.NewGuid(),
                    CompanyId = request.CompanyId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    CompanyName = request.Company,
                    JobTitle = request.Position,
                    Source = leadSource,
                    Status = leadStatus,
                    Notes = request.Notes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId.ToString(),
                    IsActive = true
                };

                // Agregar al contexto
                _context.Leads.Add(lead);

                // Guardar cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created lead with ID {LeadId} for company {CompanyId}", 
                    lead.Id, request.CompanyId);

                return lead.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lead for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}


