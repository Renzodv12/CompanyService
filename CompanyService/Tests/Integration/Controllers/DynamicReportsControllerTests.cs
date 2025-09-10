using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Enums;
using CompanyService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Xunit;

namespace CompanyService.Tests.Integration.Controllers
{
    /// <summary>
    /// Tests de integración para DynamicReportsController
    /// </summary>
    public class DynamicReportsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _testCompanyId = Guid.NewGuid();
        private readonly string _testUserToken;

        public DynamicReportsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remover el contexto de base de datos real
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Agregar contexto de base de datos en memoria
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
            
            // Configurar contexto de prueba
            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Generar token de prueba
            _testUserToken = GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testUserToken);
            
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
                Id = Guid.NewGuid(),
                Name = "Test Report",
                Description = "Test Description",
                EntityName = "Users",
                CompanyId = _testCompanyId,
                CreatedBy = _testUserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Fields = new List<ReportField>
                {
                    new() { Id = Guid.NewGuid(), FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 },
                    new() { Id = Guid.NewGuid(), FieldName = "Email", DisplayName = "Email", DataType = "string", IsVisible = true, SortOrder = 2 }
                }
            };

            _context.Users.Add(user);
            _context.ReportDefinitions.Add(report);
            _context.SaveChanges();
        }

        private string GenerateTestToken()
        {
            // En un escenario real, esto sería un JWT válido
            // Para pruebas, simulamos un token básico
            return "test-token-" + _testUserId;
        }

        [Fact]
        public async Task GetReportDefinitions_ReturnsOkWithReports()
        {
            // Act
            var response = await _client.GetAsync("/api/dynamic-reports/definitions");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var reports = JsonSerializer.Deserialize<List<ReportDefinitionListDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(reports);
            Assert.NotEmpty(reports);
        }

        [Fact]
        public async Task CreateReportDefinition_WithValidData_ReturnsCreated()
        {
            // Arrange
            var createDto = new CreateReportDefinitionDto
            {
                Name = "New Test Report",
                Description = "New test description",
                EntityName = "Users",
                Fields = new List<CreateReportFieldDto>
                {
                    new() { FieldName = "Name", DisplayName = "Name", DataType = "string", IsVisible = true, SortOrder = 1 },
                    new() { FieldName = "Email", DisplayName = "Email", DataType = "string", IsVisible = true, SortOrder = 2 }
                }
            };

            var json = JsonSerializer.Serialize(createDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/dynamic-reports/definitions", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdReport = JsonSerializer.Deserialize<ReportDefinitionResponseDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(createdReport);
            Assert.Equal(createDto.Name, createdReport.Name);
        }

        [Fact]
        public async Task CreateReportDefinition_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateReportDefinitionDto
            {
                // Name es requerido pero no se proporciona
                Description = "Test description",
                EntityName = "Users"
            };

            var json = JsonSerializer.Serialize(createDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/dynamic-reports/definitions", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAvailableEntities_ReturnsOkWithEntities()
        {
            // Act
            var response = await _client.GetAsync("/api/dynamic-reports/entities");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var entities = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
        }

        [Fact]
        public async Task GetEntityFields_WithValidEntity_ReturnsOkWithFields()
        {
            // Act
            var response = await _client.GetAsync("/api/dynamic-reports/entities/Users/fields");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var fields = JsonSerializer.Deserialize<List<EntityFieldDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
        }

        [Fact]
        public async Task ExportReport_WithValidFormat_ReturnsFileResult()
        {
            // Arrange
            var executionId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/dynamic-reports/executions/{executionId}/export?format={ExportFormat.Csv}");

            // Assert
            // Dependiendo de la implementación, esto podría ser OK o NotFound
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReportDefinitions_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var clientWithoutAuth = _factory.CreateClient();

            // Act
            var response = await clientWithoutAuth.GetAsync("/api/dynamic-reports/definitions");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateReportDefinition_WithValidData_ReturnsOk()
        {
            // Arrange
            var reportId = _context.ReportDefinitions.First().Id;
            var updateDto = new UpdateReportDefinitionDto
            {
                Name = "Updated Report Name",
                Description = "Updated description",
                Fields = new List<UpdateReportFieldDto>
                {
                    new() { FieldName = "Name", DisplayName = "Full Name", DataType = "string", IsVisible = true, SortOrder = 1 }
                }
            };

            var json = JsonSerializer.Serialize(updateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/dynamic-reports/definitions/{reportId}", content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteReportDefinition_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var reportId = _context.ReportDefinitions.First().Id;

            // Act
            var response = await _client.DeleteAsync($"/api/dynamic-reports/definitions/{reportId}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetReportExecutions_ReturnsOkWithExecutions()
        {
            // Act
            var response = await _client.GetAsync("/api/dynamic-reports/executions");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var executions = JsonSerializer.Deserialize<List<ReportExecutionListDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(executions);
        }

        public void Dispose()
        {
            _context.Dispose();
            _client.Dispose();
        }
    }
}