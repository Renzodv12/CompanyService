using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener la lista de presupuestos
    /// </summary>
    public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, List<BudgetResponseDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetBudgetsQueryHandler> _logger;

        public GetBudgetsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetBudgetsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BudgetResponseDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting budgets for company {CompanyId}", request.CompanyId);

                var query = _context.Budgets
                    .Include(b => b.Account)
                    .Include(b => b.BudgetLines)
                    .Where(b => b.CompanyId == request.CompanyId && b.IsActive);

                // Filtrar por año si se especifica
                if (request.Year.HasValue)
                {
                    query = query.Where(b => b.Year == request.Year.Value);
                }

                // Filtrar por mes si se especifica
                if (request.Month.HasValue)
                {
                    query = query.Where(b => b.Month == request.Month.Value);
                }

                var budgets = await query
                    .OrderByDescending(b => b.Year)
                    .ThenByDescending(b => b.Month)
                    .ThenBy(b => b.Name)
                    .Select(b => new BudgetResponseDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Description = b.Description,
                        Year = b.Year,
                        Month = b.Month,
                        Period = GetPeriodDescription(b.Year, b.Month),
                        StartDate = GetStartDate(b.Year, b.Month),
                        EndDate = GetEndDate(b.Year, b.Month),
                        BudgetedAmount = b.BudgetedAmount,
                        ActualAmount = b.ActualAmount,
                        Variance = b.Variance,
                        VariancePercentage = b.VariancePercentage,
                        AccountId = b.AccountId,
                        AccountName = b.Account != null ? b.Account.Name : null,
                        Category = b.Category,
                        CreatedAt = b.CreatedAt,
                        UpdatedAt = b.UpdatedAt,
                        BudgetLines = b.BudgetLines.Select(bl => new BudgetLineResponseDto
                        {
                            Id = bl.Id,
                            Description = bl.Description,
                            BudgetedAmount = bl.BudgetedAmount,
                            ActualAmount = bl.ActualAmount,
                            Variance = bl.Variance,
                            VariancePercentage = bl.BudgetedAmount != 0 ? (bl.Variance / bl.BudgetedAmount) * 100 : 0,
                            AccountId = bl.AccountId,
                            AccountName = bl.Account != null ? bl.Account.Name : null,
                            Category = bl.LineItem,
                            Notes = bl.Notes,
                            CreatedAt = bl.CreatedAt,
                            UpdatedAt = bl.UpdatedAt
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} budgets for company {CompanyId}", 
                    budgets.Count, request.CompanyId);

                return budgets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budgets for company {CompanyId}", request.CompanyId);
                throw;
            }
        }

        private static string GetPeriodDescription(int year, int? month)
        {
            if (!month.HasValue)
                return "Annual";
            
            return month.Value switch
            {
                1 => "January",
                2 => "February", 
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => $"{month:D2}/{year}"
            };
        }

        private static DateTime? GetStartDate(int year, int? month)
        {
            if (!month.HasValue)
                return new DateTime(year, 1, 1);
            
            return new DateTime(year, month.Value, 1);
        }

        private static DateTime? GetEndDate(int year, int? month)
        {
            if (!month.HasValue)
                return new DateTime(year, 12, 31);
            
            return new DateTime(year, month.Value, DateTime.DaysInMonth(year, month.Value));
        }
    }
}
