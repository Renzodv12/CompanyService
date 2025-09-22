namespace CompanyService.Core.Feature.Commands.DynamicReports
{
    /// <summary>
    /// Comando para eliminar una definición de reporte
    /// </summary>
    public class DeleteReportDefinitionCommand
    {
        /// <summary>
        /// ID de la definición del reporte a eliminar
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID del usuario que elimina el reporte
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}