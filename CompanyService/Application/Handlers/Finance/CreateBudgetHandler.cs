using CompanyService.Application.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Application.Handlers.Finance
{
    public class CreateBudgetHandler : IRequestHandler<CreateBudgetCommand, BudgetResponseDto>
    {
        private readonly IFinanceService _financeService;

        public CreateBudgetHandler(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async Task<BudgetResponseDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var dto = new CreateBudgetDto
            {
                Name = request.Name,
                Description = request.Description,
                Year = request.Year,
                Month = request.Month,
                BudgetedAmount = request.BudgetedAmount,
                Category = request.Category,
                Type = request.Type,
                Notes = request.Notes,
                CompanyId = request.CompanyId,
                UserId = request.UserId,
                BudgetLines = request.BudgetLines.Select(bl => new CreateBudgetLineDto
                {
                    AccountId = bl.AccountId,
                    Description = bl.Description,
                    BudgetedAmount = bl.BudgetedAmount,
                    Category = bl.Category,
                    Notes = bl.Notes
                }).ToList()
            };

            return await _financeService.CreateBudgetAsync(dto);
        }
    }
}