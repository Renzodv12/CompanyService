using CompanyService.Core.Enums;
using CompanyService.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace CompanyService.Tests.Unit.Services
{
    /// <summary>
    /// Tests unitarios para ReportExportService
    /// </summary>
    public class ReportExportServiceTests
    {
        private readonly Mock<ILogger<ReportExportService>> _loggerMock;
        private readonly ReportExportService _service;
        private readonly List<Dictionary<string, object>> _testData;

        public ReportExportServiceTests()
        {
            _loggerMock = new Mock<ILogger<ReportExportService>>();
            _service = new ReportExportService(_loggerMock.Object);
            
            _testData = new List<Dictionary<string, object>>
            {
                new() { { "Id", 1 }, { "Name", "John Doe" }, { "Email", "john@example.com" } },
                new() { { "Id", 2 }, { "Name", "Jane Smith" }, { "Email", "jane@example.com" } },
                new() { { "Id", 3 }, { "Name", "Bob Johnson" }, { "Email", "bob@example.com" } }
            };
        }

        [Fact]
        public async Task ExportToStreamAsync_JsonFormat_GeneratesValidJson()
        {
            // Arrange
            using var stream = new MemoryStream();
            const string reportName = "Test Report";

            // Act
            await _service.ExportToStreamAsync(_testData, ExportFormat.Json, reportName, stream);

            // Assert
            stream.Position = 0;
            var content = await new StreamReader(stream).ReadToEndAsync();
            
            Assert.NotEmpty(content);
            Assert.Contains("John Doe", content);
            Assert.Contains("jane@example.com", content);
            Assert.True(IsValidJson(content));
        }

        [Fact]
        public async Task ExportToStreamAsync_CsvFormat_GeneratesValidCsv()
        {
            // Arrange
            using var stream = new MemoryStream();
            const string reportName = "Test Report";

            // Act
            await _service.ExportToStreamAsync(_testData, ExportFormat.Csv, reportName, stream);

            // Assert
            stream.Position = 0;
            var content = await new StreamReader(stream).ReadToEndAsync();
            
            Assert.NotEmpty(content);
            Assert.Contains("Id,Name,Email", content); // Header
            Assert.Contains("John Doe", content);
            Assert.Contains("jane@example.com", content);
            
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(4, lines.Length); // Header + 3 data rows
        }

        [Fact]
        public async Task ExportToStreamAsync_ExcelFormat_GeneratesExcelFile()
        {
            // Arrange
            using var stream = new MemoryStream();
            const string reportName = "Test Report";

            // Act
            await _service.ExportToStreamAsync(_testData, ExportFormat.Excel, reportName, stream);

            // Assert
            Assert.True(stream.Length > 0);
            // Para Excel, verificamos que se haya generado contenido
            // En un test más completo, podríamos usar ClosedXML para leer y verificar el contenido
        }

        [Fact]
        public async Task ExportToStreamAsync_PdfFormat_GeneratesPdfFile()
        {
            // Arrange
            using var stream = new MemoryStream();
            const string reportName = "Test Report";

            // Act
            await _service.ExportToStreamAsync(_testData, ExportFormat.Pdf, reportName, stream);

            // Assert
            Assert.True(stream.Length > 0);
            
            // Verificar que el contenido comience con el header PDF
            stream.Position = 0;
            var buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, 4);
            var header = Encoding.ASCII.GetString(buffer);
            Assert.Equal("%PDF", header);
        }

        [Theory]
        [InlineData(ExportFormat.Json, "application/json")]
        [InlineData(ExportFormat.Csv, "text/csv")]
        [InlineData(ExportFormat.Excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [InlineData(ExportFormat.Pdf, "application/pdf")]
        public void GetContentType_ReturnsCorrectMimeType(ExportFormat format, string expectedMimeType)
        {
            // Act
            var contentType = _service.GetContentType(format);

            // Assert
            Assert.Equal(expectedMimeType, contentType);
        }

        [Theory]
        [InlineData(ExportFormat.Json, ".json")]
        [InlineData(ExportFormat.Csv, ".csv")]
        [InlineData(ExportFormat.Excel, ".xlsx")]
        [InlineData(ExportFormat.Pdf, ".pdf")]
        public void GetFileExtension_ReturnsCorrectExtension(ExportFormat format, string expectedExtension)
        {
            // Act
            var extension = _service.GetFileExtension(format);

            // Assert
            Assert.Equal(expectedExtension, extension);
        }

        [Fact]
        public async Task ExportToStreamAsync_EmptyData_GeneratesEmptyExport()
        {
            // Arrange
            using var stream = new MemoryStream();
            var emptyData = new List<Dictionary<string, object>>();
            const string reportName = "Empty Report";

            // Act
            await _service.ExportToStreamAsync(emptyData, ExportFormat.Json, reportName, stream);

            // Assert
            stream.Position = 0;
            var content = await new StreamReader(stream).ReadToEndAsync();
            Assert.Equal("[]", content.Trim());
        }

        [Fact]
        public async Task ExportToStreamAsync_NullData_ThrowsArgumentNullException()
        {
            // Arrange
            using var stream = new MemoryStream();
            const string reportName = "Test Report";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.ExportToStreamAsync(null!, ExportFormat.Json, reportName, stream));
        }

        [Fact]
        public async Task ExportToStreamAsync_NullStream_ThrowsArgumentNullException()
        {
            // Arrange
            const string reportName = "Test Report";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.ExportToStreamAsync(_testData, ExportFormat.Json, reportName, null!));
        }

        [Fact]
        public async Task ExportToStreamAsync_CsvWithSpecialCharacters_EscapesCorrectly()
        {
            // Arrange
            var specialData = new List<Dictionary<string, object>>
            {
                new() { { "Name", "John, Jr." }, { "Description", "Contains \"quotes\" and commas" } }
            };
            using var stream = new MemoryStream();
            const string reportName = "Special Chars Report";

            // Act
            await _service.ExportToStreamAsync(specialData, ExportFormat.Csv, reportName, stream);

            // Assert
            stream.Position = 0;
            var content = await new StreamReader(stream).ReadToEndAsync();
            
            Assert.Contains("\"John, Jr.\"", content);
            Assert.Contains("\"Contains \"\"quotes\"\" and commas\"", content);
        }

        private static bool IsValidJson(string jsonString)
        {
            try
            {
                System.Text.Json.JsonDocument.Parse(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}