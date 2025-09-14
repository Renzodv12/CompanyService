namespace CompanyService.Core.Enums
{
    /// <summary>
    /// Tipos de documentos que puede manejar una empresa
    /// </summary>
    public enum CompanyDocumentType
    {
        /// <summary>
        /// Documentos legales (contratos, acuerdos, etc.)
        /// </summary>
        Legal = 0,

        /// <summary>
        /// Documentos financieros (estados financieros, facturas, etc.)
        /// </summary>
        Financial = 1,

        /// <summary>
        /// Documentos operacionales (manuales, procedimientos, etc.)
        /// </summary>
        Operational = 2,

        /// <summary>
        /// Documentos de cumplimiento (certificaciones, licencias, etc.)
        /// </summary>
        Compliance = 3,

        /// <summary>
        /// Documentos de recursos humanos
        /// </summary>
        HumanResources = 4,

        /// <summary>
        /// Documentos de marketing
        /// </summary>
        Marketing = 5,

        /// <summary>
        /// Documentos técnicos
        /// </summary>
        Technical = 6,

        /// <summary>
        /// Otros documentos
        /// </summary>
        Other = 99
    }
}