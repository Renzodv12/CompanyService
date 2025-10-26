using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Commands.Restaurant
{
    // Restaurant Payment Commands
    public class CreateRestaurantPaymentCommand : IRequest<RestaurantPaymentDto>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public int Method { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class UpdateRestaurantPaymentCommand : IRequest<RestaurantPaymentDto>
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public int Method { get; set; }
        public int Status { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteRestaurantPaymentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class ProcessPaymentCommand : IRequest<RestaurantPaymentDto>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public int Method { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    public class RefundPaymentCommand : IRequest<RestaurantPaymentDto>
    {
        public Guid PaymentId { get; set; }
        public decimal RefundAmount { get; set; }
        public string? Reason { get; set; }
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}

