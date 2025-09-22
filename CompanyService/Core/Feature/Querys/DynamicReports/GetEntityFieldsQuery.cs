namespace CompanyService.Core.Feature.Querys.DynamicReports
{
    /// <summary>
    /// Query para obtener los campos de una entidad específica
    /// </summary>
    public class GetEntityFieldsQuery
    {
        /// <summary>
        /// Nombre de la entidad
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

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