using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class ReportExecution
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReportId { get; set; }
        public string Parameters { get; set; } // Parámetros usados en esta ejecución
        public DateTime ExecutionDate { get; set; } = DateTime.UtcNow;
        public string ResultData { get; set; } // Datos del resultado en JSON
        public ReportStatus Status { get; set; } = ReportStatus.Generated;
        public Guid UserId { get; set; }
        public string FilePath { get; set; } // Ruta del archivo generado (PDF, Excel, etc.)

        // Navegación
        public Report Report { get; set; }
    }
}
