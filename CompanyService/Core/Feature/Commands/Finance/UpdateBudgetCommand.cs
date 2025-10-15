using MediatR;
using CompanyService.Core.Models.Finance;

namespace CompanyService.Core.Feature.Commands.Finance
{
    /// <summary>
    /// Comando para actualizar un presupuesto existente
    /// </summary>
    public class UpdateBudgetCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; }
        public decimal BudgetedAmount { get; set; }
        public Guid? AccountId { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
        public List<UpdateBudgetLineRequest> BudgetLines { get; set; } = new();
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}