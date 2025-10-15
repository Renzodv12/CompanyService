using MediatR;
using CompanyService.Core.Feature.Querys.CRM;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.CRM;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.CRM
{
    /// <summary>
    /// Handler para obtener la lista paginada de leads
    /// </summary>
    public class GetLeadsQueryHandler : IRequestHandler<GetLeadsQuery, PagedResult<LeadDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetLeadsQueryHandler> _logger;

        public GetLeadsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetLeadsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<LeadDto>> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting leads for company {CompanyId}, page {Page}, pageSize {PageSize}", 
                    request.CompanyId, request.Page, request.PageSize);

                // Obtener el total de leads para la compañía
                var totalCount = await _context.Leads
                    .CountAsync(l => l.CompanyId == request.CompanyId, cancellationToken);

                // Obtener los leads paginados
                var leads = await _context.Leads
                    .Include(l => l.AssignedUser)
                    .Include(l => l.Company)
                    .Where(l => l.CompanyId == request.CompanyId)
                    .OrderByDescending(l => l.CreatedAt)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(l => new LeadDto
                    {
                        Id = l.Id,
                        FirstName = l.FirstName,
                        LastName = l.LastName,
                        Email = l.Email,
                        Phone = l.Phone,
                        Company = l.CompanyName,
                        JobTitle = l.JobTitle,
                        Source = l.Source,
                        Status = l.Status,
                        Notes = l.Notes,
                        NextFollowUpDate = l.NextFollowUpDate,
                        IsQualified = l.IsQualified,
                        CompanyId = l.CompanyId,
                        AssignedUserId = l.AssignedToUserId,
                        AssignedUserName = l.AssignedUser != null 
                            ? $"{l.AssignedUser.FirstName} {l.AssignedUser.LastName}".Trim()
                            : null,
                        CompanyName = l.Company.Name,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt
                    })
                    .ToListAsync(cancellationToken);

                var result = new PagedResult<LeadDto>(
                    leads,
                    totalCount,
                    request.Page,
                    request.PageSize
                );

                _logger.LogInformation("Successfully retrieved {Count} leads for company {CompanyId}", 
                    leads.Count, request.CompanyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leads for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }
}


