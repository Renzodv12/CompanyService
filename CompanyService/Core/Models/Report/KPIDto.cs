namespace CompanyService.Core.Models.Report
{
    public class KPIDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Unit { get; set; } = string.Empty; // "$", "%", "count", etc.
        public decimal Target { get; set; }
        public decimal Achievement { get; set; } // Percentage of target achieved
        public string Status { get; set; } = string.Empty; // "Good", "Warning", "Critical"
        public string Trend { get; set; } = string.Empty; // "Up", "Down", "Stable"
    }
}
