namespace CompanyService.Core.Models.Report
{
    public class RecentLeadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal EstimatedValue { get; set; }
    }
}
