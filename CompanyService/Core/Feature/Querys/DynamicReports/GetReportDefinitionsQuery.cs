namespace CompanyService.Core.Feature.Querys.DynamicReports
{
    /// <summary>
    /// Query para obtener las definiciones de reportes
    /// </summary>
    public class GetReportDefinitionsQuery
    {
        /// <summary>
        /// ID de la empresa
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ID del usuario
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Incluir reportes compartidos
        /// </summary>
        public bool IncludeShared { get; set; }
    }
}