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
    /// Tests unitarios para DynamicReportService
    /// </summary>
    public class DynamicReportServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<DynamicReportService>> _loggerMock;
        private readonly DynamicReportService _service;
        private readonly Guid _testCompanyId = Guid.NewGuid();

        public DynamicReportServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<DynamicReportService>>();
            _service = new DynamicReportService(_context, _loggerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Crear usuarios de prueba
            var users = new List<User>
            {
                new() { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com", CompanyId = _testCompanyId },
                new() { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com", CompanyId = _testCompanyId },
                new() { Id = Guid.NewGuid(), Name = "Bob Johnson", Email = "bob@example.com", CompanyId = _testCompanyId }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAvailableEntitiesAsync_ReturnsExpectedEntities()
        {
            // Act
            var entities = await _service.GetAvailableEntitiesAsync(_testCompanyId);

            // Assert
            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
            Assert.Contains("Users", entities);
        }

        [Fact]
        public async Task GetEntityFieldsAsync_WithValidEntity_ReturnsFields()
        {
            // Act
            var fields = await _service.GetEntityFieldsAsync("Users", _testCompanyId);

            // Assert
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            
            var fieldsList = fields.ToList();
            Assert.Contains(fieldsList, f => f.FieldName == "Id");
            Assert.Contains(fieldsList, f => f.FieldName == "Name");
            Assert.Contains(fieldsList, f => f.FieldName == "Email");
        }

        [Fact]
        public async Task GetEntityFieldsAsync_WithInvalidEntity_ReturnsEmpty()
        {
            // Act
            var fields = await _service.GetEntityFieldsAsync("InvalidEntity", _testCompanyId);

            // Assert
            Assert.NotNull(fields);
            Assert.Empty(fields);
        }

        [Fact]
        public async Task ExecuteReportAsync_WithValidDefinition_ReturnsData()
        {
            // Arrange
            var reportDefinition = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Users Report",
                EntityName = "Users",
                CompanyId = _testCompanyId,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Fields = new List<ReportField>
                {
                    new() { Id = Guid.NewGuid(), FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 },
                    new() { Id = Guid.NewGuid(), FieldName = "Email", DisplayName = "Email", DataType = "string", IsVisible = true, SortOrder = 2 }
                }
            };

            _context.ReportDefinitions.Add(reportDefinition);
            await _context.SaveChangesAsync();

            var executeDto = new ExecuteReportDto
            {
                ReportDefinitionId = reportDefinition.Id,
                Filters = new List<ReportFilterDto>()
            };

            // Act
            var result = await _service.ExecuteReportAsync(executeDto, _testCompanyId, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(3, result.TotalRecords); // 3 usuarios de prueba
        }

        [Fact]
        public async Task ExecuteReportAsync_WithFilters_ReturnsFilteredData()
        {
            // Arrange
            var reportDefinition = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Filtered Users Report",
                EntityName = "Users",
                CompanyId = _testCompanyId,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Fields = new List<ReportField>
                {
                    new() { Id = Guid.NewGuid(), FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 },
                    new() { Id = Guid.NewGuid(), FieldName = "Email", DisplayName = "Email", DataType = "string", IsVisible = true, SortOrder = 2 }
                }
            };

            _context.ReportDefinitions.Add(reportDefinition);
            await _context.SaveChangesAsync();

            var executeDto = new ExecuteReportDto
            {
                ReportDefinitionId = reportDefinition.Id,
                Filters = new List<ReportFilterDto>
                {
                    new() { FieldName = "Name", Operator = FilterOperator.Contains, Value = "John" }
                }
            };

            // Act
            var result = await _service.ExecuteReportAsync(executeDto, _testCompanyId, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data); // Solo un usuario con "John" en el nombre
        }

        [Fact]
        public async Task ExecuteReportAsync_WithPagination_ReturnsPagedData()
        {
            // Arrange
            var reportDefinition = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Paged Users Report",
                EntityName = "Users",
                CompanyId = _testCompanyId,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Fields = new List<ReportField>
                {
                    new() { Id = Guid.NewGuid(), FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 }
                }
            };

            _context.ReportDefinitions.Add(reportDefinition);
            await _context.SaveChangesAsync();

            var executeDto = new ExecuteReportDto
            {
                ReportDefinitionId = reportDefinition.Id,
                PageNumber = 1,
                PageSize = 2,
                Filters = new List<ReportFilterDto>()
            };

            // Act
            var result = await _service.ExecuteReportAsync(executeDto, _testCompanyId, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count()); // Página de 2 elementos
            Assert.Equal(3, result.TotalRecords); // Total de 3 registros
        }

        [Fact]
        public async Task ExecuteReportAsync_WithSorting_ReturnsSortedData()
        {
            // Arrange
            var reportDefinition = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Sorted Users Report",
                EntityName = "Users",
                CompanyId = _testCompanyId,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Fields = new List<ReportField>
                {
                    new() { Id = Guid.NewGuid(), FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 }
                }
            };

            _context.ReportDefinitions.Add(reportDefinition);
            await _context.SaveChangesAsync();

            var executeDto = new ExecuteReportDto
            {
                ReportDefinitionId = reportDefinition.Id,
                SortBy = "Name",
                SortDirection = "desc",
                Filters = new List<ReportFilterDto>()
            };

            // Act
            var result = await _service.ExecuteReportAsync(executeDto, _testCompanyId, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            
            var dataList = result.Data.ToList();
            var firstRecord = dataList.First();
            Assert.True(firstRecord.ContainsKey("Name"));
            
            // Verificar que está ordenado descendentemente
            var names = dataList.Select(d => d["Name"].ToString()).ToList();
            var sortedNames = names.OrderByDescending(n => n).ToList();
            Assert.Equal(sortedNames, names);
        }

        [Theory]
        [InlineData(FilterOperator.Equals, "John Doe", 1)]
        [InlineData(FilterOperator.Contains, "John", 1)]
        [InlineData(FilterOperator.StartsWith, "J", 2)] // John y Jane
        public async Task ExecuteReportAsync_WithDifferentOperators_ReturnsCorrectResults(
            FilterOperator filterOperator, string value, int expectedCount)
        {
            // Arrange
            var reportDefinition = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Name = "Filter Test Report",
                EntityName = "Users",
                CompanyId = _testCompanyId,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Fields = new List<ReportField>
                {
                    new() { Id = Guid.NewGuid(), FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 }
                }
            };

            _context.ReportDefinitions.Add(reportDefinition);
            await _context.SaveChangesAsync();

            var executeDto = new ExecuteReportDto
            {
                ReportDefinitionId = reportDefinition.Id,
                Filters = new List<ReportFilterDto>
                {
                    new() { FieldName = "Name", Operator = filterOperator, Value = value }
                }
            };

            // Act
            var result = await _service.ExecuteReportAsync(executeDto, _testCompanyId, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Data.Count());
        }

        [Fact]
        public async Task ValidateReportDefinitionAsync_WithValidDefinition_ReturnsTrue()
        {
            // Arrange
            var createDto = new CreateReportDefinitionDto
            {
                Name = "Valid Report",
                Description = "Valid description",
                EntityName = "Users",
                Fields = new List<CreateReportFieldDto>
                {
                    new() { FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 }
                }
            };

            // Act
            var isValid = await _service.ValidateReportDefinitionAsync(createDto, _testCompanyId);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateReportDefinitionAsync_WithInvalidEntity_ReturnsFalse()
        {
            // Arrange
            var createDto = new CreateReportDefinitionDto
            {
                Name = "Invalid Report",
                Description = "Invalid description",
                EntityName = "InvalidEntity",
                Fields = new List<CreateReportFieldDto>
                {
                    new() { FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 }
                }
            };

            // Act
            var isValid = await _service.ValidateReportDefinitionAsync(createDto, _testCompanyId);

            // Assert
            Assert.False(isValid);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}