using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Entities;
using CompanyService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static CompanyService.Core.Interfaces.IReportAuditService;
using Task = System.Threading.Tasks.Task;

namespace CompanyService.Infrastructure.Services
{
    public class ReportAuditService : IReportAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportAuditService> _logger;

        public ReportAuditService(ApplicationDbContext context, ILogger<ReportAuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogActionAsync(
            Guid userId,
            Guid companyId,
            ReportAuditAction action,
            Guid? reportId = null,
            Guid? executionId = null,
            string? details = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            await LogActionInternalAsync(userId, companyId, action, reportId, executionId, details, ipAddress, userAgent);
        }

        private async Task LogActionInternalAsync(
            Guid userId,
            Guid companyId,
            ReportAuditAction action,
            Guid? reportId = null,
            Guid? executionId = null,
            string? details = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            try
            {
                var auditLog = new ReportAuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CompanyId = companyId,
                    Action = action,
                    ReportId = reportId,
                    ExecutionId = executionId,
                    Details = details,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.ReportAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Acción de auditoría registrada: {Action} por usuario {UserId} en empresa {CompanyId}",
                    action, userId, companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al registrar acción de auditoría: {Action} por usuario {UserId}",
                    action, userId);
                throw;
            }
        }

        public async Task<IEnumerable<ReportAuditLogDto>> GetReportAuditLogAsync(
            Guid reportId,
            Guid companyId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var query = _context.ReportAuditLogs
                    .Where(log => log.ReportId == reportId && log.CompanyId == companyId);

                if (fromDate.HasValue)
                    query = query.Where(log => log.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(log => log.Timestamp <= toDate.Value);

                var logs = await query
                    .OrderByDescending(log => log.Timestamp)
                    .Include(log => log.User)
                    .Select(log => new ReportAuditLogDto
                    {
                        Id = log.Id,
                        UserId = log.UserId,
                        UserName = $"{log.User.FirstName} {log.User.LastName}".Trim(),
                        CompanyId = log.CompanyId,
                        Action = log.Action,
                        ReportId = log.ReportId,
                        ReportName = log.Report != null ? log.Report.Name : null,
                        ExecutionId = log.ExecutionId,
                        Details = log.Details,
                        IpAddress = log.IpAddress,
                        UserAgent = log.UserAgent,
                        Timestamp = log.Timestamp
                    })
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al obtener registros de auditoría para reporte {ReportId}",
                    reportId);
                throw;
            }
        }

        public async Task<IEnumerable<ReportAuditLogDto>> GetUserAuditLogAsync(
            Guid userId,
            Guid companyId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var query = _context.ReportAuditLogs
                    .Where(log => log.UserId == userId && log.CompanyId == companyId);

                if (fromDate.HasValue)
                    query = query.Where(log => log.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(log => log.Timestamp <= toDate.Value);

                var logs = await query
                    .OrderByDescending(log => log.Timestamp)
                    .Include(log => log.Report)
                    .Select(log => new ReportAuditLogDto
                    {
                        Id = log.Id,
                        UserId = log.UserId,
                        UserName = $"{log.User.FirstName} {log.User.LastName}".Trim(),
                        CompanyId = log.CompanyId,
                        Action = log.Action,
                        ReportId = log.ReportId,
                        ReportName = log.Report != null ? log.Report.Name : null,
                        ExecutionId = log.ExecutionId,
                        Details = log.Details,
                        IpAddress = log.IpAddress,
                        UserAgent = log.UserAgent,
                        Timestamp = log.Timestamp
                    })
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al obtener registros de auditoría para usuario {UserId}",
                    userId);
                throw;
            }
        }

        public async Task<ReportUsageStatsDto> GetUsageStatsAsync(
            Guid companyId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var query = _context.ReportAuditLogs
                    .Where(log => log.CompanyId == companyId);

                if (fromDate.HasValue)
                    query = query.Where(log => log.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(log => log.Timestamp <= toDate.Value);

                var logs = await query.ToListAsync();

                var stats = new ReportUsageStatsDto
                {
                    TotalReports = await _context.ReportDefinitions
                        .Where(r => r.CompanyId == companyId)
                        .CountAsync(),
                    TotalExecutions = logs.Count(l => l.Action == ReportAuditAction.ReportExecuted),
                    TotalExports = logs.Count(l => l.Action == ReportAuditAction.ReportExported),
                    ActiveUsers = logs.Select(l => l.UserId).Distinct().Count(),
                    ActionCounts = logs.GroupBy(l => l.Action.ToString())
                        .ToDictionary(g => g.Key, g => g.Count()),
                    MostUsedReports = logs.Where(l => l.ReportId.HasValue)
                        .GroupBy(l => l.ReportId!.Value)
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    ExportFormatCounts = new Dictionary<string, int>()
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al calcular estadísticas de uso para empresa {CompanyId}",
                    companyId);
                throw;
            }
        }
    }
}