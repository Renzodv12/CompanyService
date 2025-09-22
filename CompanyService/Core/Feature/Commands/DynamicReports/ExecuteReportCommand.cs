using CompanyService.Core.DTOs.DynamicReports;

namespace CompanyService.Core.Feature.Commands.DynamicReports
{
    /// <summary>
    /// Comando para ejecutar un reporte
    /// </summary>
    public class ExecuteReportCommand
    {
        /// <summary>
        /// Datos para la ejecución del reporte
        /// </summary>
        public ExecuteReportDto ExecuteReportDto { get; set; } = new();

        /// <summary>
        /// ID del usuario que ejecuta el reporte
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}