using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para eliminar un lead
    /// </summary>
    public class DeleteLeadCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}