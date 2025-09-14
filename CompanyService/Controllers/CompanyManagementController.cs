using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CompanyService.Core.Interfaces;
using CompanyService.Core.DTOs.Branch;
using CompanyService.Core.DTOs.Department;
using CompanyService.Core.DTOs.CompanySettings;
using CompanyService.Core.DTOs.CompanyDocument;
using System.Security.Claims;

namespace CompanyService.Controllers
{
    /// <summary>
    /// Controlador para gestión avanzada de empresas
    /// </summary>
    [ApiController]
    [Route("api/companies/{companyId:guid}/management")]
    [Authorize]
    public class CompanyManagementController : ControllerBase
    {
        private readonly ICompanyManagementService _managementService;
        private readonly ILogger<CompanyManagementController> _logger;

        public CompanyManagementController(
            ICompanyManagementService managementService,
            ILogger<CompanyManagementController> logger)
        {
            _managementService = managementService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private string? GetUserAgent()
        {
            return HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
        }

        #region Branch Management
        /// <summary>
        /// Obtiene todas las sucursales de una empresa
        /// </summary>
        [HttpGet("branches")]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches(Guid companyId)
        {
            try
            {
                var branches = await _managementService.GetBranchesAsync(companyId);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sucursales de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una sucursal específica
        /// </summary>
        [HttpGet("branches/{branchId:guid}")]
        public async Task<ActionResult<BranchDto>> GetBranch(Guid companyId, Guid branchId)
        {
            try
            {
                var branch = await _managementService.GetBranchByIdAsync(companyId, branchId);
                if (branch == null)
                    return NotFound("Sucursal no encontrada");

                return Ok(branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sucursal {BranchId} de la empresa {CompanyId}", branchId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva sucursal
        /// </summary>
        [HttpPost("branches")]
        public async Task<ActionResult<object>> CreateBranch(Guid companyId, [FromBody] CreateBranchRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var branchId = await _managementService.CreateBranchAsync(companyId, request, userId);

                return CreatedAtAction(nameof(GetBranch), new { companyId, branchId }, new { id = branchId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear sucursal en la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una sucursal existente
        /// </summary>
        [HttpPut("branches/{branchId:guid}")]
        public async Task<ActionResult> UpdateBranch(Guid companyId, Guid branchId, [FromBody] UpdateBranchRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                await _managementService.UpdateBranchAsync(companyId, branchId, request, userId);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar sucursal {BranchId} de la empresa {CompanyId}", branchId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una sucursal
        /// </summary>
        [HttpDelete("branches/{branchId:guid}")]
        public async Task<ActionResult> DeleteBranch(Guid companyId, Guid branchId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _managementService.DeleteBranchAsync(companyId, branchId, userId);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar sucursal {BranchId} de la empresa {CompanyId}", branchId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
        #endregion

        #region Department Management
        /// <summary>
        /// Obtiene todos los departamentos de una empresa
        /// </summary>
        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments(Guid companyId)
        {
            try
            {
                var departments = await _managementService.GetDepartmentsAsync(companyId);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamentos de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la jerarquía de departamentos
        /// </summary>
        [HttpGet("departments/hierarchy")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartmentHierarchy(Guid companyId)
        {
            try
            {
                var hierarchy = await _managementService.GetDepartmentHierarchyAsync(companyId);
                return Ok(hierarchy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jerarquía de departamentos de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un departamento específico
        /// </summary>
        [HttpGet("departments/{departmentId:guid}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(Guid companyId, Guid departmentId)
        {
            try
            {
                var department = await _managementService.GetDepartmentByIdAsync(companyId, departmentId);
                if (department == null)
                    return NotFound("Departamento no encontrado");

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamento {DepartmentId} de la empresa {CompanyId}", departmentId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo departamento
        /// </summary>
        [HttpPost("departments")]
        public async Task<ActionResult<object>> CreateDepartment(Guid companyId, [FromBody] CreateDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var departmentId = await _managementService.CreateDepartmentAsync(companyId, request, userId);

                return CreatedAtAction(nameof(GetDepartment), new { companyId, departmentId }, new { id = departmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear departamento en la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un departamento existente
        /// </summary>
        [HttpPut("departments/{departmentId:guid}")]
        public async Task<ActionResult> UpdateDepartment(Guid companyId, Guid departmentId, [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                await _managementService.UpdateDepartmentAsync(companyId, departmentId, request, userId);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar departamento {DepartmentId} de la empresa {CompanyId}", departmentId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un departamento
        /// </summary>
        [HttpDelete("departments/{departmentId:guid}")]
        public async Task<ActionResult> DeleteDepartment(Guid companyId, Guid departmentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _managementService.DeleteDepartmentAsync(companyId, departmentId, userId);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar departamento {DepartmentId} de la empresa {CompanyId}", departmentId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
        #endregion

        #region Company Settings
        /// <summary>
        /// Obtiene las configuraciones de la empresa
        /// </summary>
        [HttpGet("settings")]
        public async Task<ActionResult<CompanySettingsDto>> GetSettings(Guid companyId)
        {
            try
            {
                var settings = await _managementService.GetCompanySettingsAsync(companyId);
                if (settings == null)
                {
                    // Inicializar configuraciones por defecto
                    var userId = GetCurrentUserId();
                    await _managementService.InitializeDefaultSettingsAsync(companyId, userId);
                    settings = await _managementService.GetCompanySettingsAsync(companyId);
                }

                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuraciones de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza las configuraciones de la empresa
        /// </summary>
        [HttpPut("settings")]
        public async Task<ActionResult> UpdateSettings(Guid companyId, [FromBody] UpdateCompanySettingsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                await _managementService.UpdateCompanySettingsAsync(companyId, request, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuraciones de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
        #endregion

        #region Document Management
        /// <summary>
        /// Obtiene todos los documentos de la empresa
        /// </summary>
        [HttpGet("documents")]
        public async Task<ActionResult<IEnumerable<CompanyDocumentDto>>> GetDocuments(Guid companyId)
        {
            try
            {
                var documents = await _managementService.GetCompanyDocumentsAsync(companyId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener documentos de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un documento específico
        /// </summary>
        [HttpGet("documents/{documentId:guid}")]
        public async Task<ActionResult<CompanyDocumentDto>> GetDocument(Guid companyId, Guid documentId)
        {
            try
            {
                var document = await _managementService.GetCompanyDocumentByIdAsync(companyId, documentId);
                if (document == null)
                    return NotFound("Documento no encontrado");

                return Ok(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener documento {DocumentId} de la empresa {CompanyId}", documentId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Sube un nuevo documento
        /// </summary>
        [HttpPost("documents")]
        public async Task<ActionResult<object>> UploadDocument(Guid companyId, [FromForm] UploadCompanyDocumentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var documentId = await _managementService.UploadCompanyDocumentAsync(companyId, request, userId);

                return CreatedAtAction(nameof(GetDocument), new { companyId, documentId }, new { id = documentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir documento a la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Descarga un documento
        /// </summary>
        [HttpGet("documents/{documentId:guid}/download")]
        public async Task<ActionResult> DownloadDocument(Guid companyId, Guid documentId)
        {
            try
            {
                var (content, fileName, mimeType) = await _managementService.DownloadCompanyDocumentAsync(companyId, documentId);
                return File(content, mimeType, fileName);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar documento {DocumentId} de la empresa {CompanyId}", documentId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un documento
        /// </summary>
        [HttpDelete("documents/{documentId:guid}")]
        public async Task<ActionResult> DeleteDocument(Guid companyId, Guid documentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _managementService.DeleteCompanyDocumentAsync(companyId, documentId, userId);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar documento {DocumentId} de la empresa {CompanyId}", documentId, companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
        #endregion

        #region Audit Log
        /// <summary>
        /// Obtiene el log de auditoría de la empresa
        /// </summary>
        [HttpGet("audit-log")]
        public async Task<ActionResult<IEnumerable<object>>> GetAuditLog(
            Guid companyId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var auditLogs = await _managementService.GetCompanyAuditLogAsync(companyId, fromDate, toDate);
                
                var result = auditLogs.Select(log => new
                {
                    log.Id,
                    log.Action,
                    log.EntityType,
                    log.EntityId,
                    log.EntityName,
                    log.OldValues,
                    log.NewValues,
                    log.IpAddress,
                    log.UserAgent,
                    log.Level,
                    log.CreatedAt,
                    UserName = log.User != null ? $"{log.User.FirstName} {log.User.LastName}" : "Sistema"
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener log de auditoría de la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
        #endregion
    }
}