namespace CompanyService.Core.Feature.Querys.DynamicReports
{
    /// <summary>
    /// Query para obtener una definición de reporte específica
    /// </summary>
    public class GetReportDefinitionQuery
    {
        /// <summary>
        /// ID de la definición del reporte
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ID del usuario
        /// </summary>
        public Guid UserId { get; set; }
    }
}