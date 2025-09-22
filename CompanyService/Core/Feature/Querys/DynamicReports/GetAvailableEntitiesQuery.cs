namespace CompanyService.Core.Feature.Querys.DynamicReports
{
    /// <summary>
    /// Query para obtener las entidades disponibles para reportes
    /// </summary>
    public class GetAvailableEntitiesQuery
    {
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