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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<UpdateBudgetLineRequest> Lines { get; set; } = new();
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}