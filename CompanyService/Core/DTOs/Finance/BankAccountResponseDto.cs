namespace CompanyService.Core.DTOs.Finance
{
    public class BankAccountResponseDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? AccountType { get; set; }
        public string? SwiftCode { get; set; }
        public string? RoutingNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TransactionCount { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }
}