using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para duplicar un presupuesto existente
    /// </summary>
    public class DuplicateBudgetCommandHandler : IRequestHandler<DuplicateBudgetCommand, BudgetResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DuplicateBudgetCommandHandler> _logger;

        public DuplicateBudgetCommandHandler(
            ApplicationDbContext context,
            ILogger<DuplicateBudgetCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BudgetResponseDto> Handle(DuplicateBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Duplicating budget {BudgetId} for company {CompanyId}",
                    request.Id, request.CompanyId);

                // Buscar el presupuesto original
                var originalBudget = await _context.Budgets
                    .Include(b => b.BudgetLines)
                    .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId && b.IsActive, cancellationToken);

                if (originalBudget == null)
                {
                    throw new ArgumentException("Presupuesto no encontrado");
                }

                // Crear el nuevo presupuesto
                var newBudget = new Budget
                {
                    Id = Guid.NewGuid(),
                    Name = request.NewName,
                    Description = originalBudget.Description,
                    Year = request.NewYear ?? originalBudget.Year,
                    Month = request.NewMonth ?? originalBudget.Month,
                    BudgetedAmount = originalBudget.BudgetedAmount,
                    ActualAmount = 0, // Nuevo presupuesto empieza con 0
                    AccountId = originalBudget.AccountId,
                    Category = originalBudget.Category,
                    Notes = originalBudget.Notes,
                    CompanyId = request.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Duplicar las líneas de presupuesto
                var newBudgetLines = originalBudget.BudgetLines.Select(bl => new BudgetLine
                {
                    Id = Guid.NewGuid(),
                    BudgetId = newBudget.Id,
                    Description = bl.Description,
                    BudgetedAmount = bl.BudgetedAmount,
                    ActualAmount = 0, // Nuevas líneas empiezan con 0
                    AccountId = bl.AccountId,
                    LineItem = bl.LineItem,
                    Notes = bl.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                // Guardar el nuevo presupuesto y sus líneas
                _context.Budgets.Add(newBudget);
                _context.BudgetLines.AddRange(newBudgetLines);
                await _context.SaveChangesAsync(cancellationToken);

                // Recargar el presupuesto para obtener los valores completos
                var duplicatedBudget = await _context.Budgets
                    .Include(b => b.Account)
                    .Include(b => b.BudgetLines)
                        .ThenInclude(bl => bl.Account)
                    .Where(b => b.Id == newBudget.Id && b.CompanyId == request.CompanyId && b.IsActive)
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
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (duplicatedBudget == null)
                {
                    throw new InvalidOperationException("No se pudo recuperar el presupuesto duplicado.");
                }

                _logger.LogInformation("Budget {BudgetId} duplicated successfully as {NewBudgetId} for company {CompanyId}",
                    request.Id, duplicatedBudget.Id, request.CompanyId);

                return duplicatedBudget;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error duplicating budget {BudgetId} for company {CompanyId}",
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
