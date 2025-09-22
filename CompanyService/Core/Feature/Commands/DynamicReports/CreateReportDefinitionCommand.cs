using CompanyService.Core.DTOs.DynamicReports;

namespace CompanyService.Core.Feature.Commands.DynamicReports
{
    /// <summary>
    /// Comando para crear una nueva definición de reporte
    /// </summary>
    public class CreateReportDefinitionCommand
    {
        /// <summary>
        /// Datos de la definición del reporte a crear
        /// </summary>
        public CreateReportDefinitionDto ReportDefinition { get; set; } = new();

        /// <summary>
        /// ID del usuario que crea el reporte
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}