using MediatR;

namespace CompanyService.Core.Feature.Commands.Sale
{
    /// <summary>
    /// Comando para eliminar una compra existente
    /// </summary>
    public class DeletePurchaseCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}

