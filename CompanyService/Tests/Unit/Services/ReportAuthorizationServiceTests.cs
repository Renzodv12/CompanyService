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
    /// Tests unitarios para ReportAuthorizationService
    /// </summary>
    public class ReportAuthorizationServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<ReportAuthorizationService>> _loggerMock;
        private readonly ReportAuthorizationService _service;
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _testCompanyId = Guid.NewGuid();
        private readonly Guid _testReportId = Guid.NewGuid();

        public ReportAuthorizationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<ReportAuthorizationService>>();
            _service = new ReportAuthorizationService(_context, _loggerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Crear usuario de prueba
            var user = new User
            {
                Id = _testUserId,
                Name = "Test User",
                Email = "test@example.com",
                CompanyId = _testCompanyId
            };

            // Crear reporte de prueba
            var report = new ReportDefinition
            {
                Id = _testReportId,
                Name = "Test Report",
                Description = "Test Description",
                CompanyId = _testCompanyId,
                CreatedBy = _testUserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            _context.ReportDefinitions.Add(report);
            _context.SaveChanges();
        }

        [Fact]
        public async Task HasPermissionAsync_WithValidPermission_ReturnsTrue()
        {
            // Act
            var result = await _service.HasPermissionAsync(_testUserId, _testCompanyId, ReportPermission.ViewReports);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task HasPermissionAsync_WithInvalidUser_ReturnsFalse()
        {
            // Arrange
            var invalidUserId = Guid.NewGuid();

            // Act
            var result = await _service.HasPermissionAsync(invalidUserId, _testCompanyId, ReportPermission.ViewReports);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanAccessReportAsync_AsOwner_ReturnsTrue()
        {
            // Act
            var result = await _service.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId, ReportPermission.ViewReports);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CanAccessReportAsync_NonExistentReport_ReturnsFalse()
        {
            // Arrange
            var nonExistentReportId = Guid.NewGuid();

            // Act
            var result = await _service.CanAccessReportAsync(_testUserId, _testCompanyId, nonExistentReportId, ReportPermission.ViewReports);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanViewExecutionAsync_WithValidExecution_ReturnsTrue()
        {
            // Arrange
            var execution = new ReportExecution
            {
                Id = Guid.NewGuid(),
                ReportDefinitionId = _testReportId,
                ExecutedBy = _testUserId,
                CompanyId = _testCompanyId,
                ExecutedAt = DateTime.UtcNow,
                Status = "Completed"
            };

            _context.ReportExecutions.Add(execution);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CanViewExecutionAsync(_testUserId, _testCompanyId, execution.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_ReturnsExpectedPermissions()
        {
            // Act
            var permissions = await _service.GetUserPermissionsAsync(_testUserId, _testCompanyId);

            // Assert
            Assert.NotNull(permissions);
            Assert.Contains(ReportPermission.ViewReports, permissions);
        }

        [Fact]
        public async Task CanCreateReportWithEntityAsync_WithValidEntity_ReturnsTrue()
        {
            // Act
            var result = await _service.CanCreateReportWithEntityAsync(_testUserId, _testCompanyId, "Users");

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(ReportPermission.ViewReports)]
        [InlineData(ReportPermission.CreateReports)]
        [InlineData(ReportPermission.ExecuteReports)]
        public async Task HasPermissionAsync_WithDifferentPermissions_ReturnsExpectedResult(ReportPermission permission)
        {
            // Act
            var result = await _service.HasPermissionAsync(_testUserId, _testCompanyId, permission);

            // Assert
            // En este test básico, asumimos que todos los permisos están disponibles
            Assert.True(result);
        }

        [Fact]
        public async Task CanAccessReportAsync_WithDifferentCompany_ReturnsFalse()
        {
            // Arrange
            var differentCompanyId = Guid.NewGuid();

            // Act
            var result = await _service.CanAccessReportAsync(_testUserId, differentCompanyId, _testReportId, ReportPermission.ViewReports);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}