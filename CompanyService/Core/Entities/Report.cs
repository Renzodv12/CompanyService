using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public ReportType Type { get; set; }
        public string Parameters { get; set; } // JSON con parámetros del reporte
        public bool IsActive { get; set; } = true;
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Company Company { get; set; }
        public ICollection<ReportExecution> ReportExecutions { get; set; } = new List<ReportExecution>();
    }
}
