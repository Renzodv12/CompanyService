namespace CompanyService.Core.Feature.Querys.DynamicReports
{
    /// <summary>
    /// Query para obtener las ejecuciones de reportes
    /// </summary>
    public class GetReportExecutionsQuery
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
        /// Número de página
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Tamaño de página
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}