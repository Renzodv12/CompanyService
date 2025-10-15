using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para eliminar un presupuesto
    /// </summary>
    public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteBudgetCommandHandler> _logger;

        public DeleteBudgetCommandHandler(
            ApplicationDbContext context,
            ILogger<DeleteBudgetCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                // Buscar el presupuesto
                var budget = await _context.Budgets
                    .Include(b => b.BudgetLines)
                    .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, cancellationToken);

                if (budget == null)
                {
                    throw new ArgumentException("Presupuesto no encontrado");
                }

                // Nota: En el futuro se puede agregar validación de transacciones asociadas

                // Eliminar líneas de presupuesto
                if (budget.BudgetLines.Any())
                {
                    _context.BudgetLines.RemoveRange(budget.BudgetLines);
                }

                // Eliminar el presupuesto
                _context.Budgets.Remove(budget);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting budget {BudgetId} for company {CompanyId}", 
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
