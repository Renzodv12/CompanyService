using CompanyService.Core.DTOs.Finance;
using MediatR;

namespace CompanyService.Application.Commands.Finance
{
    public class CreateBudgetCommand : IRequest<BudgetResponseDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal BudgetedAmount { get; set; }
        public string? Category { get; set; }
        public Guid CompanyId { get; set; }
        public List<CreateBudgetLineCommand> BudgetLines { get; set; } = new();
    }

    public class CreateBudgetLineCommand
    {
        public string Description { get; set; } = string.Empty;
        public decimal BudgetedAmount { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
    }
}