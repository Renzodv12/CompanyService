using MediatR;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para obtener un presupuesto específico
    /// </summary>
    public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, BudgetResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetBudgetQueryHandler> _logger;

        public GetBudgetQueryHandler(
            ApplicationDbContext context,
            ILogger<GetBudgetQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BudgetResponseDto> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                var budget = await _context.Budgets
                    .Include(b => b.Account)
                    .Include(b => b.BudgetLines)
                        .ThenInclude(bl => bl.Account)
                    .Where(b => b.Id == request.Id && b.CompanyId == request.CompanyId && b.IsActive)
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
                        Notes = b.Notes,
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
                        }).OrderBy(bl => bl.CreatedAt).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (budget == null)
                {
                    throw new ArgumentException("Presupuesto no encontrado");
                }

                // Calcular métricas adicionales si no están calculadas
                if (budget.BudgetLines.Any())
                {
                    var totalBudgetedLines = budget.BudgetLines.Sum(bl => bl.BudgetedAmount);
                    var totalActualLines = budget.BudgetLines.Sum(bl => bl.ActualAmount);
                    
                    // Si el presupuesto principal no tiene datos calculados, calcularlos
                    if (budget.ActualAmount == 0 && totalActualLines > 0)
                    {
                        budget.ActualAmount = totalActualLines;
                        budget.Variance = budget.ActualAmount - budget.BudgetedAmount;
                        budget.VariancePercentage = budget.BudgetedAmount != 0 
                            ? (budget.Variance / budget.BudgetedAmount) * 100 
                            : 0;
                    }
                }

                _logger.LogInformation("Successfully retrieved budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                return budget;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
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
