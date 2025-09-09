using CompanyService.Core.Handlers.Menu;
using CompanyService.Core.Models.Menu;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.Endpoints
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMediator mediator, ILogger<MenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los menús disponibles en el sistema
        /// </summary>
        /// <returns>Lista de todos los menús activos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<MenuDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<MenuDto>>> GetAllMenus()
        {
            try
            {
                var query = new GetAllMenusQuery();
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los menús");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los menús configurados para una empresa específica
        /// </summary>
        /// <param name="companyId">ID de la empresa</param>
        /// <returns>Lista de menús configurados para la empresa</returns>
        [HttpGet("company/{companyId}")]
        [ProducesResponseType(typeof(List<MenuDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<MenuDto>>> GetMenusByCompany(Guid companyId)
        {
            try
            {
                if (companyId == Guid.Empty)
                {
                    return BadRequest("El ID de la empresa es requerido");
                }

                var query = new GetMenusByCompanyQuery(companyId);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menús para empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la configuración actual de menús para una empresa
        /// </summary>
        /// <param name="companyId">ID de la empresa</param>
        /// <returns>Configuración de menús de la empresa</returns>
        [HttpGet("configuration/{companyId}")]
        [ProducesResponseType(typeof(List<CompanyMenuConfigurationDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<CompanyMenuConfigurationDto>>> GetCompanyMenuConfiguration(Guid companyId)
        {
            try
            {
                if (companyId == Guid.Empty)
                {
                    return BadRequest("El ID de la empresa es requerido");
                }

                var query = new GetCompanyMenuConfigurationQuery(companyId);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener configuración de menús para empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza la configuración de menús para una empresa
        /// </summary>
        /// <param name="request">Datos de configuración de menús</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("configuration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateCompanyMenuConfiguration([FromBody] UpdateCompanyMenuConfigurationRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Los datos de configuración son requeridos");
                }

                if (request.CompanyId == Guid.Empty)
                {
                    return BadRequest("El ID de la empresa es requerido");
                }

                if (request.MenuConfigurations == null || !request.MenuConfigurations.Any())
                {
                    return BadRequest("Debe especificar al menos una configuración de menú");
                }

                var command = new UpdateCompanyMenuConfigurationCommand(request);
                var result = await _mediator.Send(command);
                
                if (result)
                {
                    return Ok(new { message = "Configuración de menús actualizada exitosamente" });
                }
                else
                {
                    return StatusCode(500, "Error al actualizar la configuración de menús");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuración de menús para empresa {CompanyId}", request?.CompanyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}