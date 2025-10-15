using MediatR;

namespace CompanyService.Core.Feature.Commands.Finance
{
    /// <summary>
    /// Comando para eliminar un presupuesto
    /// </summary>
    public class DeleteBudgetCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}


