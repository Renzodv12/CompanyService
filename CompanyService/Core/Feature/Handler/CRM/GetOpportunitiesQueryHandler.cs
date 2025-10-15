using MediatR;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para obtener la lista paginada de oportunidades
    /// </summary>
    public class GetOpportunitiesQueryHandler : IRequestHandler<GetOpportunitiesQuery, PagedResult<OpportunityDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetOpportunitiesQueryHandler> _logger;

        public GetOpportunitiesQueryHandler(
            ApplicationDbContext context,
            ILogger<GetOpportunitiesQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<OpportunityDto>> Handle(GetOpportunitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting opportunities for company {CompanyId}, page {Page}, pageSize {PageSize}", 
                    request.CompanyId, request.Page, request.PageSize);

                // Obtener el total de oportunidades para la compañía
                var totalCount = await _context.Opportunities
                    .CountAsync(o => o.CompanyId == request.CompanyId, cancellationToken);

                // Obtener las oportunidades paginadas
                var opportunities = await _context.Opportunities
                    .Include(o => o.AssignedUser)
                    .Include(o => o.Lead)
                    .Include(o => o.Customer)
                    .Where(o => o.CompanyId == request.CompanyId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(o => new OpportunityDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Description = o.Description,
                        Stage = o.Stage,
                        Value = o.EstimatedValue,
                        Probability = o.Probability,
                        ExpectedCloseDate = o.ExpectedCloseDate,
                        ActualCloseDate = o.ActualCloseDate,
                        Notes = o.Notes,
                        CompanyId = o.CompanyId,
                        LeadId = o.LeadId,
                        AssignedUserId = o.AssignedToUserId,
                        AssignedUserName = o.AssignedUser != null 
                            ? $"{o.AssignedUser.FirstName} {o.AssignedUser.LastName}".Trim()
                            : null,
                        LeadName = o.Lead != null 
                            ? $"{o.Lead.FirstName} {o.Lead.LastName}".Trim()
                            : null,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt
                    })
                    .ToListAsync(cancellationToken);

                var result = new PagedResult<OpportunityDto>(
                    opportunities,
                    totalCount,
                    request.Page,
                    request.PageSize
                );

                _logger.LogInformation("Successfully retrieved {Count} opportunities for company {CompanyId}", 
                    opportunities.Count, request.CompanyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting opportunities for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}
