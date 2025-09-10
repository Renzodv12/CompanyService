using CompanyService.Core.Enums;

namespace CompanyService.Core.Interfaces
{
    public interface IReportExportService
    {
        /// <summary>
        /// Exporta los datos de un reporte a un stream en el formato especificado
        /// </summary>
        /// <param name="data">Datos del reporte a exportar</param>
        /// <param name="format">Formato de exportación</param>
        /// <param name="reportName">Nombre del reporte</param>
        /// <param name="stream">Stream donde escribir los datos exportados</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task ExportToStreamAsync(IEnumerable<Dictionary<string, object>> data, ExportFormat format, string reportName, Stream stream);

        /// <summary>
        /// Obtiene el tipo de contenido MIME para el formato especificado
        /// </summary>
        /// <param name="format">Formato de exportación</param>
        /// <returns>Tipo de contenido MIME</returns>
        string GetContentType(ExportFormat format);

        /// <summary>
        /// Obtiene la extensión de archivo para el formato especificado
        /// </summary>
        /// <param name="format">Formato de exportación</param>
        /// <returns>Extensión de archivo</returns>
        string GetFileExtension(ExportFormat format);
    }
}