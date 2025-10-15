using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Commands.Finance
{
    /// <summary>
    /// Comando para duplicar un presupuesto existente
    /// </summary>
    public class DuplicateBudgetCommand : IRequest<BudgetResponseDto>
    {
        public Guid Id { get; set; }
        public string NewName { get; set; } = string.Empty;
        public int? NewYear { get; set; }
        public int? NewMonth { get; set; }
        public DateTime? NewStartDate { get; set; }
        public DateTime? NewEndDate { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}
