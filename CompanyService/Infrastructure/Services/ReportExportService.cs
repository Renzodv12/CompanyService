using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using System.Text;
using System.Text.Json;
using ExportFormatEnum = CompanyService.Core.Enums.ExportFormat;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;

namespace CompanyService.Infrastructure.Services
{
    public class ReportExportService : IReportExportService
    {
        private readonly ILogger<ReportExportService> _logger;

        public ReportExportService(ILogger<ReportExportService> logger)
        {
            _logger = logger;
        }

        public async Task ExportToStreamAsync(IEnumerable<Dictionary<string, object>> data, ExportFormatEnum format, string reportName, Stream stream)
        {
            try
            {
                switch (format)
                {
                    case ExportFormatEnum.Json:
                        await ExportToJsonAsync(data, stream);
                        break;
                    case ExportFormatEnum.Csv:
                        await ExportToCsvAsync(data, stream);
                        break;
                    case ExportFormatEnum.Excel:
                        await ExportToExcelAsync(data, reportName, stream);
                        break;
                    case ExportFormatEnum.Pdf:
                        await ExportToPdfAsync(data, reportName, stream);
                        break;
                    default:
                        throw new ArgumentException($"Formato de exportación no soportado: {format}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to {Format}", format);
                throw;
            }
        }

        public string GetContentType(ExportFormatEnum format)
        {
            return format switch
            {
                ExportFormatEnum.Json => "application/json",
                ExportFormatEnum.Csv => "text/csv",
                ExportFormatEnum.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ExportFormatEnum.Pdf => "application/pdf",
                _ => throw new ArgumentException($"Formato no soportado: {format}")
            };
        }

        public string GetFileExtension(ExportFormatEnum format)
        {
            return format switch
            {
                ExportFormatEnum.Json => ".json",
                ExportFormatEnum.Csv => ".csv",
                ExportFormatEnum.Excel => ".xlsx",
                ExportFormatEnum.Pdf => ".pdf",
                _ => throw new ArgumentException($"Formato no soportado: {format}")
            };
        }

        private async Task ExportToJsonAsync(IEnumerable<Dictionary<string, object>> data, Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await JsonSerializer.SerializeAsync(stream, data, options);
        }

        private async Task ExportToCsvAsync(IEnumerable<Dictionary<string, object>> data, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
            
            var dataList = data.ToList();
            if (!dataList.Any()) return;

            // Escribir encabezados
            var headers = dataList.First().Keys.ToList();
            var headerLine = string.Join(",", headers.Select(EscapeCsvField));
            await writer.WriteLineAsync(headerLine);

            // Escribir datos
            foreach (var row in dataList)
            {
                var values = headers.Select(header => 
                    row.ContainsKey(header) ? row[header]?.ToString() ?? "" : "");
                var dataLine = string.Join(",", values.Select(EscapeCsvField));
                await writer.WriteLineAsync(dataLine);
            }
        }

        private async Task ExportToExcelAsync(IEnumerable<Dictionary<string, object>> data, string reportName, Stream stream)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(reportName);
            
            var dataList = data.ToList();
            if (!dataList.Any()) 
            {
                workbook.SaveAs(stream);
                return;
            }

            // Agregar encabezados
            var headers = dataList.First().Keys.ToList();
            for (int i = 0; i < headers.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Agregar datos
            for (int row = 0; row < dataList.Count; row++)
            {
                var rowData = dataList[row];
                for (int col = 0; col < headers.Count; col++)
                {
                    var header = headers[col];
                    var value = rowData.ContainsKey(header) ? rowData[header] : null;
                    worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                }
            }

            // Ajustar ancho de columnas
            worksheet.ColumnsUsed().AdjustToContents();
            
            workbook.SaveAs(stream);
        }

        private async Task ExportToPdfAsync(IEnumerable<Dictionary<string, object>> data, string reportName, Stream stream)
        {
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Título del reporte
            document.Add(new Paragraph(reportName)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(16)
                .SetBold());

            var dataList = data.ToList();
            if (!dataList.Any()) return;

            // Crear tabla
            var headers = dataList.First().Keys.ToList();
            var table = new Table(headers.Count);
            table.SetWidth(UnitValue.CreatePercentValue(100));

            // Agregar encabezados
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(header))
                    .SetBold()
                    .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY));
            }

            // Agregar datos
            foreach (var row in dataList)
            {
                foreach (var header in headers)
                {
                    var value = row.ContainsKey(header) ? row[header]?.ToString() ?? "" : "";
                    table.AddCell(new Cell().Add(new Paragraph(value)));
                }
            }

            document.Add(table);
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // Si el campo contiene comas, comillas o saltos de línea, debe estar entre comillas
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                // Escapar comillas duplicándolas
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }
    }
}