using CompanyService.Application.Commands.DynamicReports;
using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.Attributes;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CompanyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DynamicReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DynamicReportsController> _logger;

        public DynamicReportsController(IMediator mediator, ILogger<DynamicReportsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private Guid GetCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            return Guid.TryParse(companyIdClaim, out var companyId) ? companyId : Guid.Empty;
        }

        /// <summary>
        /// Obtiene todas las definiciones de reportes para la empresa
        /// </summary>
        [HttpGet("definitions")]
        [ReportPermission(ReportPermission.ViewReports)]
        public async Task<ActionResult<IEnumerable<ReportDefinitionListDto>>> GetReportDefinitions(
            [FromQuery] bool includeShared = true)
        {
            try
            {
                var query = new GetReportDefinitionsQuery
                {
                    CompanyId = GetCompanyId(),
                    UserId = GetUserId(),
                    IncludeShared = includeShared
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report definitions");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una definición de reporte específica
        /// </summary>
        [HttpGet("definitions/{id}")]
        [ReportPermission(ReportPermission.ViewReports, requireOwnership: true)]
        public async Task<ActionResult<ReportDefinitionResponseDto>> GetReportDefinition(Guid id)
        {
            try
            {
                var query = new GetReportDefinitionQuery
                {
                    Id = id,
                    CompanyId = GetCompanyId(),
                    UserId = GetUserId()
                };

                var result = await _mediator.Send(query);
                
                if (result == null)
                {
                    return NotFound("Definición de reporte no encontrada");
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("No tiene permisos para acceder a este reporte");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report definition {ReportId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva definición de reporte
        /// </summary>
        [HttpPost("definitions")]
        [ReportPermission(ReportPermission.CreateReports)]
        public async Task<ActionResult<ReportDefinitionResponseDto>> CreateReportDefinition(
            [FromBody] CreateReportDefinitionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new CreateReportDefinitionCommand
                {
                    ReportDefinition = dto,
                    UserId = GetUserId(),
                    CompanyId = GetCompanyId()
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetReportDefinition), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report definition");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una definición de reporte existente
        /// </summary>
        [HttpPut("definitions/{id}")]
        [ReportPermission(ReportPermission.EditOwnReports, requireOwnership: true)]
        public async Task<ActionResult<ReportDefinitionResponseDto>> UpdateReportDefinition(
            Guid id, [FromBody] UpdateReportDefinitionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new UpdateReportDefinitionCommand
                {
                    Id = id,
                    ReportDefinition = dto,
                    UserId = GetUserId(),
                    CompanyId = GetCompanyId()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report definition {ReportId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una definición de reporte
        /// </summary>
        [HttpDelete("definitions/{id}")]
        [ReportPermission(ReportPermission.DeleteOwnReports, requireOwnership: true)]
        public async Task<ActionResult> DeleteReportDefinition(Guid id)
        {
            try
            {
                var command = new DeleteReportDefinitionCommand
                {
                    Id = id,
                    UserId = GetUserId(),
                    CompanyId = GetCompanyId()
                };

                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return NotFound("Definición de reporte no encontrada");
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report definition {ReportId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Ejecuta un reporte dinámico
        /// </summary>
        [HttpPost("execute")]
        [ReportPermission(ReportPermission.ExecuteReports)]
        public async Task<ActionResult<ReportExecutionResponseDto>> ExecuteReport(
            [FromBody] ExecuteReportDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new ExecuteReportCommand
                {
                    ExecuteRequest = dto,
                    UserId = GetUserId()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing report");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial de ejecuciones de reportes
        /// </summary>
        [HttpGet("executions")]
        [ReportPermission(ReportPermission.ViewExecutionHistory)]
        public async Task<ActionResult<IEnumerable<ReportExecutionListDto>>> GetReportExecutions(
            [FromQuery] Guid? filterByUserId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = new GetReportExecutionsQuery
                {
                    CompanyId = GetCompanyId(),
                    UserId = GetUserId(),
                    FilterByUserId = filterByUserId,
                    PageNumber = pageNumber,
                    PageSize = Math.Min(pageSize, 100) // Limitar el tamaño de página
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report executions");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las entidades disponibles para crear reportes
        /// </summary>
        [HttpGet("entities")]
        [ReportPermission(ReportPermission.ViewReports)]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableEntities()
        {
            try
            {
                var query = new GetAvailableEntitiesQuery
                {
                    CompanyId = GetCompanyId(),
                    UserId = GetUserId()
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available entities");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los campos disponibles para una entidad específica
        /// </summary>
        [HttpGet("entities/{entityName}/fields")]
        [ReportPermission(ReportPermission.ViewReports)]
        public async Task<ActionResult<IEnumerable<EntityFieldDto>>> GetEntityFields(string entityName)
        {
            try
            {
                var query = new GetEntityFieldsQuery
                {
                    EntityName = entityName,
                    CompanyId = GetCompanyId(),
                    UserId = GetUserId()
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity fields for {EntityName}", entityName);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Exporta los resultados de una ejecución de reporte
        /// </summary>
        [HttpGet("executions/{executionId}/export")]
        [ReportPermission(ReportPermission.ExportReports)]
        public async Task<ActionResult> ExportReport(Guid executionId, [FromQuery] Core.Enums.ExportFormat format = Core.Enums.ExportFormat.Json)
        {
            try
            {
                // TODO: Implementar obtención de datos de ejecución desde la base de datos
                // Por ahora retornamos datos de ejemplo
                var sampleData = new List<Dictionary<string, object>>
                {
                    new() { ["Id"] = 1, ["Nombre"] = "Producto A", ["Precio"] = 100.50m, ["Fecha"] = DateTime.Now },
                    new() { ["Id"] = 2, ["Nombre"] = "Producto B", ["Precio"] = 200.75m, ["Fecha"] = DateTime.Now.AddDays(-1) }
                };

                var reportName = $"Reporte_{executionId:N}";
                var fileName = $"{reportName}_{DateTime.Now:yyyyMMdd_HHmmss}";

                var exportService = HttpContext.RequestServices.GetRequiredService<IReportExportService>();
                var contentType = exportService.GetContentType(format);
                var fileExtension = exportService.GetFileExtension(format);

                var stream = new MemoryStream();
                await exportService.ExportToStreamAsync(sampleData, format, reportName, stream);
                stream.Position = 0;

                return File(stream, contentType, $"{fileName}{fileExtension}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report execution {ExecutionId}", executionId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}