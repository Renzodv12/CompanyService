using CompanyService.Core.DTOs.DynamicReports;

namespace CompanyService.Core.Feature.Commands.DynamicReports
{
    /// <summary>
    /// Comando para actualizar una definición de reporte existente
    /// </summary>
    public class UpdateReportDefinitionCommand
    {
        /// <summary>
        /// ID de la definición del reporte a actualizar
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Datos actualizados de la definición del reporte
        /// </summary>
        public UpdateReportDefinitionDto ReportDefinition { get; set; } = new();

        /// <summary>
        /// ID del usuario que actualiza el reporte
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}