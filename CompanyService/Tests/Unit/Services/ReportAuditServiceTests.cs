using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Enums;
using CompanyService.Infrastructure.Data;
using CompanyService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CompanyService.Tests.Unit.Services
{
    /// <summary>
    /// Tests unitarios para ReportAuditService
    /// </summary>
    public class ReportAuditServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<ReportAuditService>> _loggerMock;
        private readonly ReportAuditService _service;
        private readonly Guid _testCompanyId = Guid.NewGuid();
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _testReportId = Guid.NewGuid();

        public ReportAuditServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<ReportAuditService>>();
            _service = new ReportAuditService(_context, _loggerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Crear datos de auditoría de prueba
            var auditLogs = new List<ReportAuditLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReportId = _testReportId,
                    UserId = _testUserId,
                    CompanyId = _testCompanyId,
                    Action = ReportAuditAction.ReportCreated,
                    Timestamp = DateTime.UtcNow.AddDays(-5),
                    Details = "Report created successfully"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ReportId = _testReportId,
                    UserId = _testUserId,
                    CompanyId = _testCompanyId,
                    Action = ReportAuditAction.ReportExecuted,
                    Timestamp = DateTime.UtcNow.AddDays(-3),
                    Details = "Report executed with 100 records"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ReportId = _testReportId,
                    UserId = _testUserId,
                    CompanyId = _testCompanyId,
                    Action = ReportAuditAction.ReportExported,
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Details = "Report exported to CSV"
                }
            };

            _context.Set<ReportAuditLog>().AddRange(auditLogs);
            _context.SaveChanges();
        }

        [Fact]
        public async Task LogActionAsync_WithValidData_CreatesAuditLog()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var action = ReportAuditAction.ReportUpdated;
            var details = "Report updated successfully";

            // Act
            await _service.LogActionAsync(reportId, userId, _testCompanyId, action, details);

            // Assert
            var auditLog = await _context.Set<ReportAuditLog>()
                .FirstOrDefaultAsync(a => a.ReportId == reportId && a.UserId == userId && a.Action == action);

            Assert.NotNull(auditLog);
            Assert.Equal(reportId, auditLog.ReportId);
            Assert.Equal(userId, auditLog.UserId);
            Assert.Equal(_testCompanyId, auditLog.CompanyId);
            Assert.Equal(action, auditLog.Action);
            Assert.Equal(details, auditLog.Details);
            Assert.True(auditLog.Timestamp > DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task LogActionAsync_WithNullDetails_CreatesAuditLogWithNullDetails()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var action = ReportAuditAction.ReportDeleted;

            // Act
            await _service.LogActionAsync(reportId, userId, _testCompanyId, action, null);

            // Assert
            var auditLog = await _context.Set<ReportAuditLog>()
                .FirstOrDefaultAsync(a => a.ReportId == reportId && a.UserId == userId && a.Action == action);

            Assert.NotNull(auditLog);
            Assert.Null(auditLog.Details);
        }

        [Fact]
        public async Task GetReportAuditLogsAsync_WithValidReportId_ReturnsAuditLogs()
        {
            // Act
            var auditLogs = await _service.GetReportAuditLogsAsync(_testReportId, _testCompanyId);

            // Assert
            Assert.NotNull(auditLogs);
            Assert.Equal(3, auditLogs.Count());
            
            var logsList = auditLogs.ToList();
            Assert.All(logsList, log => Assert.Equal(_testReportId, log.ReportId));
            Assert.All(logsList, log => Assert.Equal(_testCompanyId, log.CompanyId));
            
            // Verificar que están ordenados por timestamp descendente
            var timestamps = logsList.Select(l => l.Timestamp).ToList();
            var sortedTimestamps = timestamps.OrderByDescending(t => t).ToList();
            Assert.Equal(sortedTimestamps, timestamps);
        }

        [Fact]
        public async Task GetReportAuditLogsAsync_WithNonExistentReportId_ReturnsEmpty()
        {
            // Arrange
            var nonExistentReportId = Guid.NewGuid();

            // Act
            var auditLogs = await _service.GetReportAuditLogsAsync(nonExistentReportId, _testCompanyId);

            // Assert
            Assert.NotNull(auditLogs);
            Assert.Empty(auditLogs);
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_WithValidUserId_ReturnsAuditLogs()
        {
            // Act
            var auditLogs = await _service.GetUserAuditLogsAsync(_testUserId, _testCompanyId);

            // Assert
            Assert.NotNull(auditLogs);
            Assert.Equal(3, auditLogs.Count());
            
            var logsList = auditLogs.ToList();
            Assert.All(logsList, log => Assert.Equal(_testUserId, log.UserId));
            Assert.All(logsList, log => Assert.Equal(_testCompanyId, log.CompanyId));
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_WithNonExistentUserId_ReturnsEmpty()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();

            // Act
            var auditLogs = await _service.GetUserAuditLogsAsync(nonExistentUserId, _testCompanyId);

            // Assert
            Assert.NotNull(auditLogs);
            Assert.Empty(auditLogs);
        }

        [Fact]
        public async Task GetUsageStatsAsync_WithValidCompanyId_ReturnsStats()
        {
            // Act
            var stats = await _service.GetUsageStatsAsync(_testCompanyId);

            // Assert
            Assert.NotNull(stats);
            Assert.Equal(3, stats.TotalActions);
            Assert.Equal(1, stats.TotalReports);
            Assert.Equal(1, stats.TotalUsers);
            Assert.True(stats.MostActiveUsers.Any());
            Assert.True(stats.MostUsedReports.Any());
            Assert.True(stats.ActionBreakdown.Any());
        }

        [Fact]
        public async Task GetUsageStatsAsync_WithDateRange_ReturnsFilteredStats()
        {
            // Arrange
            var fromDate = DateTime.UtcNow.AddDays(-4);
            var toDate = DateTime.UtcNow.AddDays(-2);

            // Act
            var stats = await _service.GetUsageStatsAsync(_testCompanyId, fromDate, toDate);

            // Assert
            Assert.NotNull(stats);
            Assert.Equal(1, stats.TotalActions); // Solo el log de ReportExecuted está en el rango
            Assert.Equal(1, stats.TotalReports);
            Assert.Equal(1, stats.TotalUsers);
        }

        [Fact]
        public async Task GetUsageStatsAsync_WithEmptyCompany_ReturnsZeroStats()
        {
            // Arrange
            var emptyCompanyId = Guid.NewGuid();

            // Act
            var stats = await _service.GetUsageStatsAsync(emptyCompanyId);

            // Assert
            Assert.NotNull(stats);
            Assert.Equal(0, stats.TotalActions);
            Assert.Equal(0, stats.TotalReports);
            Assert.Equal(0, stats.TotalUsers);
            Assert.Empty(stats.MostActiveUsers);
            Assert.Empty(stats.MostUsedReports);
            Assert.Empty(stats.ActionBreakdown);
        }

        [Theory]
        [InlineData(ReportAuditAction.ReportCreated)]
        [InlineData(ReportAuditAction.ReportUpdated)]
        [InlineData(ReportAuditAction.ReportDeleted)]
        [InlineData(ReportAuditAction.ReportExecuted)]
        [InlineData(ReportAuditAction.ReportExported)]
        [InlineData(ReportAuditAction.ReportAccessDenied)]
        public async Task LogActionAsync_WithDifferentActions_CreatesCorrectAuditLog(ReportAuditAction action)
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var details = $"Test action: {action}";

            // Act
            await _service.LogActionAsync(reportId, userId, _testCompanyId, action, details);

            // Assert
            var auditLog = await _context.Set<ReportAuditLog>()
                .FirstOrDefaultAsync(a => a.ReportId == reportId && a.Action == action);

            Assert.NotNull(auditLog);
            Assert.Equal(action, auditLog.Action);
            Assert.Equal(details, auditLog.Details);
        }

        [Fact]
        public async Task GetReportAuditLogsAsync_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            // Agregar más logs para probar paginación
            var additionalLogs = new List<ReportAuditLog>();
            for (int i = 0; i < 10; i++)
            {
                additionalLogs.Add(new ReportAuditLog
                {
                    Id = Guid.NewGuid(),
                    ReportId = _testReportId,
                    UserId = _testUserId,
                    CompanyId = _testCompanyId,
                    Action = ReportAuditAction.ReportExecuted,
                    Timestamp = DateTime.UtcNow.AddHours(-i),
                    Details = $"Additional log {i}"
                });
            }

            _context.Set<ReportAuditLog>().AddRange(additionalLogs);
            await _context.SaveChangesAsync();

            // Act
            var auditLogs = await _service.GetReportAuditLogsAsync(_testReportId, _testCompanyId, pageNumber: 1, pageSize: 5);

            // Assert
            Assert.NotNull(auditLogs);
            Assert.Equal(5, auditLogs.Count());
        }

        [Fact]
        public async Task GetUsageStatsAsync_VerifiesActionBreakdown()
        {
            // Act
            var stats = await _service.GetUsageStatsAsync(_testCompanyId);

            // Assert
            Assert.NotNull(stats.ActionBreakdown);
            Assert.Contains(stats.ActionBreakdown, kvp => kvp.Key == "ReportCreated" && kvp.Value == 1);
            Assert.Contains(stats.ActionBreakdown, kvp => kvp.Key == "ReportExecuted" && kvp.Value == 1);
            Assert.Contains(stats.ActionBreakdown, kvp => kvp.Key == "ReportExported" && kvp.Value == 1);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}