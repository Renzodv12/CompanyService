using CompanyService.Core.DTOs.Branch;
using CompanyService.Core.DTOs.Department;
using CompanyService.Core.DTOs.CompanySettings;
using CompanyService.Core.DTOs.CompanyDocument;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de gestión avanzada de empresas
    /// </summary>
    public interface ICompanyManagementService
    {
        #region Branch Management
        /// <summary>
        /// Obtiene todas las sucursales de una empresa
        /// </summary>
        System.Threading.Tasks.Task<IEnumerable<BranchDto>> GetBranchesAsync(Guid companyId);

        /// <summary>
        /// Obtiene una sucursal por su ID
        /// </summary>
        System.Threading.Tasks.Task<BranchDto?> GetBranchByIdAsync(Guid companyId, Guid branchId);

        /// <summary>
        /// Crea una nueva sucursal
        /// </summary>
        System.Threading.Tasks.Task<Guid> CreateBranchAsync(Guid companyId, CreateBranchRequest request, Guid userId);

        /// <summary>
        /// Actualiza una sucursal existente
        /// </summary>
        System.Threading.Tasks.Task UpdateBranchAsync(Guid companyId, Guid branchId, UpdateBranchRequest request, Guid userId);

        /// <summary>
        /// Elimina una sucursal (soft delete)
        /// </summary>
        System.Threading.Tasks.Task DeleteBranchAsync(Guid companyId, Guid branchId, Guid userId);
        #endregion

        #region Department Management
        /// <summary>
        /// Obtiene todos los departamentos de una empresa
        /// </summary>
        System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(Guid companyId);

        /// <summary>
        /// Obtiene un departamento por su ID
        /// </summary>
        System.Threading.Tasks.Task<DepartmentDto?> GetDepartmentByIdAsync(Guid companyId, Guid departmentId);

        /// <summary>
        /// Crea un nuevo departamento
        /// </summary>
        System.Threading.Tasks.Task<Guid> CreateDepartmentAsync(Guid companyId, CreateDepartmentRequest request, Guid userId);

        /// <summary>
        /// Actualiza un departamento existente
        /// </summary>
        System.Threading.Tasks.Task UpdateDepartmentAsync(Guid companyId, Guid departmentId, UpdateDepartmentRequest request, Guid userId);

        /// <summary>
        /// Elimina un departamento (soft delete)
        /// </summary>
        System.Threading.Tasks.Task DeleteDepartmentAsync(Guid companyId, Guid departmentId, Guid userId);

        /// <summary>
        /// Obtiene la jerarquía de departamentos
        /// </summary>
        System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> GetDepartmentHierarchyAsync(Guid companyId);
        #endregion

        #region Company Settings
        /// <summary>
        /// Obtiene las configuraciones de una empresa
        /// </summary>
        System.Threading.Tasks.Task<CompanySettingsDto?> GetCompanySettingsAsync(Guid companyId);

        /// <summary>
        /// Actualiza las configuraciones de una empresa
        /// </summary>
        System.Threading.Tasks.Task UpdateCompanySettingsAsync(Guid companyId, UpdateCompanySettingsRequest request, Guid userId);

        /// <summary>
        /// Inicializa las configuraciones por defecto para una empresa
        /// </summary>
        System.Threading.Tasks.Task InitializeDefaultSettingsAsync(Guid companyId, Guid userId);
        #endregion

        #region Document Management
        /// <summary>
        /// Obtiene todos los documentos de una empresa
        /// </summary>
        System.Threading.Tasks.Task<IEnumerable<CompanyDocumentDto>> GetCompanyDocumentsAsync(Guid companyId);

        /// <summary>
        /// Obtiene un documento por su ID
        /// </summary>
        System.Threading.Tasks.Task<CompanyDocumentDto?> GetCompanyDocumentByIdAsync(Guid companyId, Guid documentId);

        /// <summary>
        /// Sube un nuevo documento
        /// </summary>
        System.Threading.Tasks.Task<Guid> UploadCompanyDocumentAsync(Guid companyId, UploadCompanyDocumentRequest request, Guid userId);

        /// <summary>
        /// Elimina un documento (soft delete)
        /// </summary>
        System.Threading.Tasks.Task DeleteCompanyDocumentAsync(Guid companyId, Guid documentId, Guid userId);

        /// <summary>
        /// Descarga un documento
        /// </summary>
        System.Threading.Tasks.Task<(byte[] content, string fileName, string mimeType)> DownloadCompanyDocumentAsync(Guid companyId, Guid documentId);
        #endregion

        #region Audit and Logging
        /// <summary>
        /// Obtiene los logs de auditoría de una empresa
        /// </summary>
        System.Threading.Tasks.Task<IEnumerable<AuditLog>> GetCompanyAuditLogAsync(Guid companyId, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Registra una acción en el log de auditoría
        /// </summary>
        System.Threading.Tasks.Task LogAuditActionAsync(Guid companyId, Guid? userId, string action, string entityType, Guid? entityId, 
            string? entityName, object? oldValues, object? newValues, string ipAddress, string? userAgent);
        #endregion
    }
}