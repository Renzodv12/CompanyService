using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para actualizar un presupuesto existente
    /// </summary>
    public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateBudgetCommandHandler> _logger;

        public UpdateBudgetCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateBudgetCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                // Buscar el presupuesto existente
                var budget = await _context.Budgets
                    .Include(b => b.BudgetLines)
                    .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, cancellationToken);

                if (budget == null)
                {
                    throw new ArgumentException("Presupuesto no encontrado");
                }

                // Actualizar propiedades del presupuesto
                budget.Name = request.Name;
                budget.Description = request.Description;
                budget.Year = request.Year;
                budget.Month = request.Month;
                budget.BudgetedAmount = request.BudgetedAmount;
                budget.AccountId = request.AccountId;
                budget.Category = request.Category ?? string.Empty;
                budget.Notes = request.Notes ?? string.Empty;
                budget.UpdatedAt = DateTime.UtcNow;

                // Eliminar líneas existentes
                if (budget.BudgetLines.Any())
                {
                    _context.BudgetLines.RemoveRange(budget.BudgetLines);
                }

                // Agregar nuevas líneas
                if (request.BudgetLines?.Any() == true)
                {
                    var newBudgetLines = request.BudgetLines.Select((bl, index) => new BudgetLine
                    {
                        Id = bl.Id ?? Guid.NewGuid(),
                        BudgetId = budget.Id,
                        LineItem = bl.Description,
                        Description = bl.Description,
                        BudgetedAmount = bl.BudgetedAmount,
                        ActualAmount = 0,
                        AccountId = bl.AccountId,
                        Notes = bl.Notes ?? string.Empty,
                        SortOrder = index + 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }).ToList();

                    await _context.BudgetLines.AddRangeAsync(newBudgetLines, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
