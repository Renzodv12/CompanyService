using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener todas las cuentas contables
    /// </summary>
    public class GetChartOfAccountsQueryHandler : IRequestHandler<GetChartOfAccountsQuery, List<ChartOfAccountsDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetChartOfAccountsQueryHandler> _logger;

        public GetChartOfAccountsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetChartOfAccountsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ChartOfAccountsDto>> Handle(GetChartOfAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting chart of accounts for company {CompanyId}", request.CompanyId);

                var query = _context.ChartOfAccounts
                    .Include(c => c.Parent)
                    .Where(c => c.CompanyId == request.CompanyId);

                if (request.IsActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == request.IsActive.Value);
                }

                if (!string.IsNullOrEmpty(request.Type))
                {
                    query = query.Where(c => c.Type == request.Type);
                }

                var accounts = await query
                    .OrderBy(c => c.Type)
                    .ThenBy(c => c.Code)
                    .Select(c => new ChartOfAccountsDto
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Type = c.Type,
                        ParentId = c.ParentId,
                        ParentName = c.Parent != null ? c.Parent.Name : null,
                        IsActive = c.IsActive,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync(cancellationToken);

                // Organizar jerarquía
                var result = BuildHierarchy(accounts);

                _logger.LogInformation("Successfully retrieved {Count} chart of accounts entries for company {CompanyId}",
                    accounts.Count, request.CompanyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chart of accounts for company {CompanyId}", request.CompanyId);
                throw;
            }
        }

        private static List<ChartOfAccountsDto> BuildHierarchy(List<ChartOfAccountsDto> accounts)
        {
            var accountDict = accounts.ToDictionary(a => a.Id);
            var rootAccounts = new List<ChartOfAccountsDto>();

            foreach (var account in accounts)
            {
                if (account.ParentId == null)
                {
                    rootAccounts.Add(account);
                }
                else if (accountDict.ContainsKey(account.ParentId.Value))
                {
                    var parent = accountDict[account.ParentId.Value];
                    parent.Children.Add(account);
                }
            }

            return rootAccounts;
        }
    }

    /// <summary>
    /// Handler para obtener una cuenta contable específica
    /// </summary>
    public class GetChartOfAccountsByIdQueryHandler : IRequestHandler<GetChartOfAccountsByIdQuery, ChartOfAccountsDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetChartOfAccountsByIdQueryHandler> _logger;

        public GetChartOfAccountsByIdQueryHandler(
            ApplicationDbContext context,
            ILogger<GetChartOfAccountsByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChartOfAccountsDto> Handle(GetChartOfAccountsByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);

                var account = await _context.ChartOfAccounts
                    .Include(c => c.Parent)
                    .Where(c => c.Id == request.Id && c.CompanyId == request.CompanyId)
                    .Select(c => new ChartOfAccountsDto
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Type = c.Type,
                        ParentId = c.ParentId,
                        ParentName = c.Parent != null ? c.Parent.Name : null,
                        IsActive = c.IsActive,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (account == null)
                {
                    throw new ArgumentException("Cuenta contable no encontrada");
                }

                _logger.LogInformation("Successfully retrieved chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);

                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}

