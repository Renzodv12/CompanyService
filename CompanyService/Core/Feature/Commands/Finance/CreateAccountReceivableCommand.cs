using MediatR;

namespace CompanyService.Core.Feature.Commands.Finance
{
    public class CreateAccountReceivableCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}