namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Formatos de exportación disponibles para reportes
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// Formato JSON
        /// </summary>
        Json = 0,

        /// <summary>
        /// Formato CSV (Comma Separated Values)
        /// </summary>
        Csv = 1,

        /// <summary>
        /// Formato Excel (XLSX)
        /// </summary>
        Excel = 2,

        /// <summary>
        /// Formato PDF
        /// </summary>
        Pdf = 3
    }
}