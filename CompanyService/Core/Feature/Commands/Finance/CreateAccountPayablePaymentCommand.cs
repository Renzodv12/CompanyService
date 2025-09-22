using MediatR;

namespace CompanyService.Core.Feature.Commands.Finance
{
    public class CreateAccountPayablePaymentCommand : IRequest<Guid>
    {
        public Guid AccountPayableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}