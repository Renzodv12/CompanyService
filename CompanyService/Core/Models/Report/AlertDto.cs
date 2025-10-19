namespace CompanyService.Core.Models.Report
{
    public class AlertDto
    {
        public string Type { get; set; } = string.Empty; // "Warning", "Critical", "Info"
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string ActionUrl { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty;
    }
}
