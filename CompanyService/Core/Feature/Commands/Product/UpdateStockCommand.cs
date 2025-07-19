using CompanyService.Core.Enums;
using MediatR;

namespace CompanyService.Core.Feature.Commands.Product
{
    public class UpdateStockCommand : IRequest<bool>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public MovementType MovementType { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
