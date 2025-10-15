using MediatR;

namespace CompanyService.Core.Feature.Commands.Finance
{
    /// <summary>
    /// Comando para actualizar una cuenta por pagar existente
    /// </summary>
    public class UpdateAccountPayableCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}

