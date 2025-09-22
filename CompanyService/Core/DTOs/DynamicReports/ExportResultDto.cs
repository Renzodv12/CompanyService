namespace CompanyService.Core.DTOs.DynamicReports
{
    /// <summary>
    /// DTO para el resultado de exportación de reportes
    /// </summary>
    public class ExportResultDto
    {
        /// <summary>
        /// Contenido del archivo exportado
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Tipo de contenido del archivo
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del archivo
        /// </summary>
        public string FileName { get; set; } = string.Empty;
    }
}