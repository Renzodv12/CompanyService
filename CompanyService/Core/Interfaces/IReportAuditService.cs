using CompanyService.Core.Enums;

namespace CompanyService.Core.Interfaces
{
    /// <summary>
    /// Servicio para auditoría de acciones en reportes dinámicos
    /// </summary>
    public interface IReportAuditService
    {
        /// <summary>
        /// Registra una acción realizada en un reporte
        /// </summary>
        /// <param name="userId">ID del usuario que realizó la acción</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="action">Acción realizada</param>
        /// <param name="reportId">ID del reporte (opcional)</param>
        /// <param name="executionId">ID de la ejecución (opcional)</param>
        /// <param name="details">Detalles adicionales de la acción</param>
        /// <param name="ipAddress">Dirección IP del usuario</param>
        /// <param name="userAgent">User agent del navegador</param>
        Task LogActionAsync(
            Guid userId,
            Guid companyId,
            ReportAuditAction action,
            Guid? reportId = null,
            Guid? executionId = null,
            string? details = null,
            string? ipAddress = null,
            string? userAgent = null);

        /// <summary>
        /// Obtiene el historial de auditoría para un reporte específico
        /// </summary>
        /// <param name="reportId">ID del reporte</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de registros de auditoría</returns>
        Task<IEnumerable<ReportAuditLogDto>> GetReportAuditLogAsync(
            Guid reportId,
            Guid companyId,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Obtiene el historial de auditoría para un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de registros de auditoría</returns>
        Task<IEnumerable<ReportAuditLogDto>> GetUserAuditLogAsync(
            Guid userId,
            Guid companyId,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Obtiene estadísticas de uso de reportes
        /// </summary>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Estadísticas de uso</returns>
        Task<ReportUsageStatsDto> GetUsageStatsAsync(
            Guid companyId,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }

    /// <summary>
    /// DTO para registros de auditoría de reportes
    /// </summary>
    public class ReportAuditLogDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public ReportAuditAction Action { get; set; }
        public Guid? ReportId { get; set; }
        public string? ReportName { get; set; }
        public Guid? ExecutionId { get; set; }
        public string? Details { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// DTO para estadísticas de uso de reportes
    /// </summary>
    public class ReportUsageStatsDto
    {
        public int TotalReports { get; set; }
        public int TotalExecutions { get; set; }
        public int TotalExports { get; set; }
        public int ActiveUsers { get; set; }
        public Dictionary<string, int> ActionCounts { get; set; } = new();
        public Dictionary<string, int> MostUsedReports { get; set; } = new();
        public Dictionary<string, int> ExportFormatCounts { get; set; } = new();
    }
}