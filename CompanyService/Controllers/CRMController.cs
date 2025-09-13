using CompanyService.Core.DTOs.CRM;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CompanyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CRMController : ControllerBase
    {
        private readonly ICRMService _crmService;
        private readonly ILogger<CRMController> _logger;

        public CRMController(ICRMService crmService, ILogger<CRMController> logger)
        {
            _crmService = crmService;
            _logger = logger;
        }

        #region Lead Endpoints

        /// <summary>
        /// Obtiene todos los leads de una empresa
        /// </summary>
        [HttpGet("leads")]
        public async Task<ActionResult<IEnumerable<LeadListDto>>> GetLeads(
            [FromQuery] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var leads = await _crmService.GetLeadsAsync(companyId, page, pageSize);
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener leads para la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un lead específico por ID
        /// </summary>
        [HttpGet("leads/{id}")]
        public async Task<ActionResult<LeadDto>> GetLead(Guid id)
        {
            try
            {
                var lead = await _crmService.GetLeadByIdAsync(id);
                if (lead == null)
                    return NotFound($"Lead con ID {id} no encontrado");

                return Ok(lead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lead {LeadId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo lead
        /// </summary>
        [HttpPost("leads")]
        public async Task<ActionResult<LeadDto>> CreateLead([FromBody] CreateLeadDto createLeadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var lead = await _crmService.CreateLeadAsync(createLeadDto);
                return CreatedAtAction(nameof(GetLead), new { id = lead.Id }, lead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear lead");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un lead existente
        /// </summary>
        [HttpPut("leads/{id}")]
        public async Task<ActionResult<LeadDto>> UpdateLead(Guid id, [FromBody] UpdateLeadDto updateLeadDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var lead = await _crmService.UpdateLeadAsync(id, updateLeadDto);
                if (lead == null)
                    return NotFound($"Lead con ID {id} no encontrado");

                return Ok(lead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar lead {LeadId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un lead
        /// </summary>
        [HttpDelete("leads/{id}")]
        public async Task<ActionResult> DeleteLead(Guid id)
        {
            try
            {
                var result = await _crmService.DeleteLeadAsync(id);
                if (!result)
                    return NotFound($"Lead con ID {id} no encontrado");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lead {LeadId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Convierte un lead en una oportunidad
        /// </summary>
        [HttpPost("leads/{id}/convert")]
        public async Task<ActionResult<OpportunityDto>> ConvertLeadToOpportunity(
            Guid id, 
            [FromBody] CreateOpportunityDto createOpportunityDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var opportunity = await _crmService.ConvertLeadToOpportunityAsync(id, createOpportunityDto);
                if (opportunity == null)
                    return NotFound($"Lead con ID {id} no encontrado");

                return Ok(opportunity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al convertir lead {LeadId} a oportunidad", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion

        #region Opportunity Endpoints

        /// <summary>
        /// Obtiene todas las oportunidades de una empresa
        /// </summary>
        [HttpGet("opportunities")]
        public async Task<ActionResult<IEnumerable<OpportunityListDto>>> GetOpportunities(
            [FromQuery] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var opportunities = await _crmService.GetOpportunitiesAsync(companyId, page, pageSize);
                return Ok(opportunities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener oportunidades para la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una oportunidad específica por ID
        /// </summary>
        [HttpGet("opportunities/{id}")]
        public async Task<ActionResult<OpportunityDto>> GetOpportunity(Guid id)
        {
            try
            {
                var opportunity = await _crmService.GetOpportunityByIdAsync(id);
                if (opportunity == null)
                    return NotFound($"Oportunidad con ID {id} no encontrada");

                return Ok(opportunity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener oportunidad {OpportunityId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva oportunidad
        /// </summary>
        [HttpPost("opportunities")]
        public async Task<ActionResult<OpportunityDto>> CreateOpportunity([FromBody] CreateOpportunityDto createOpportunityDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var opportunity = await _crmService.CreateOpportunityAsync(createOpportunityDto);
                return CreatedAtAction(nameof(GetOpportunity), new { id = opportunity.Id }, opportunity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear oportunidad");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una oportunidad existente
        /// </summary>
        [HttpPut("opportunities/{id}")]
        public async Task<ActionResult<OpportunityDto>> UpdateOpportunity(Guid id, [FromBody] UpdateOpportunityDto updateOpportunityDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var opportunity = await _crmService.UpdateOpportunityAsync(id, updateOpportunityDto);
                if (opportunity == null)
                    return NotFound($"Oportunidad con ID {id} no encontrada");

                return Ok(opportunity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar oportunidad {OpportunityId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza el stage de una oportunidad
        /// </summary>
        [HttpPatch("opportunities/{id}/stage")]
        public async Task<ActionResult<OpportunityDto>> UpdateOpportunityStage(
            Guid id, 
            [FromBody] OpportunityStageUpdateDto stageUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var opportunity = await _crmService.UpdateOpportunityStageAsync(id, stageUpdateDto);
                if (opportunity == null)
                    return NotFound($"Oportunidad con ID {id} no encontrada");

                return Ok(opportunity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar stage de oportunidad {OpportunityId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una oportunidad
        /// </summary>
        [HttpDelete("opportunities/{id}")]
        public async Task<ActionResult> DeleteOpportunity(Guid id)
        {
            try
            {
                var result = await _crmService.DeleteOpportunityAsync(id);
                if (!result)
                    return NotFound($"Oportunidad con ID {id} no encontrada");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar oportunidad {OpportunityId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion

        #region Campaign Endpoints

        /// <summary>
        /// Obtiene todas las campañas de una empresa
        /// </summary>
        [HttpGet("campaigns")]
        public async Task<ActionResult<IEnumerable<CampaignListDto>>> GetCampaigns(
            [FromQuery] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var campaigns = await _crmService.GetCampaignsAsync(companyId, page, pageSize);
                return Ok(campaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener campañas para la empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una campaña específica por ID
        /// </summary>
        [HttpGet("campaigns/{id}")]
        public async Task<ActionResult<CampaignDto>> GetCampaign(Guid id)
        {
            try
            {
                var campaign = await _crmService.GetCampaignByIdAsync(id);
                if (campaign == null)
                    return NotFound($"Campaña con ID {id} no encontrada");

                return Ok(campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener campaña {CampaignId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva campaña
        /// </summary>
        [HttpPost("campaigns")]
        public async Task<ActionResult<CampaignDto>> CreateCampaign([FromBody] CreateCampaignDto createCampaignDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var campaign = await _crmService.CreateCampaignAsync(createCampaignDto);
                return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear campaña");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una campaña existente
        /// </summary>
        [HttpPut("campaigns/{id}")]
        public async Task<ActionResult<CampaignDto>> UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto updateCampaignDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var campaign = await _crmService.UpdateCampaignAsync(id, updateCampaignDto);
                if (campaign == null)
                    return NotFound($"Campaña con ID {id} no encontrada");

                return Ok(campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar campaña {CampaignId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una campaña
        /// </summary>
        [HttpDelete("campaigns/{id}")]
        public async Task<ActionResult> DeleteCampaign(Guid id)
        {
            try
            {
                var result = await _crmService.DeleteCampaignAsync(id);
                if (!result)
                    return NotFound($"Campaña con ID {id} no encontrada");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar campaña {CampaignId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de una campaña
        /// </summary>
        [HttpGet("campaigns/{id}/stats")]
        public async Task<ActionResult<CampaignStatsDto>> GetCampaignStats(Guid id)
        {
            try
            {
                var stats = await _crmService.GetCampaignStatsAsync(id);
                if (stats == null)
                    return NotFound($"Campaña con ID {id} no encontrada");

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de campaña {CampaignId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Asocia un lead a una campaña
        /// </summary>
        [HttpPost("campaigns/{campaignId}/leads/{leadId}")]
        public async Task<ActionResult> AddLeadToCampaign(Guid campaignId, Guid leadId)
        {
            try
            {
                var result = await _crmService.AddLeadToCampaignAsync(campaignId, leadId);
                if (!result)
                    return NotFound("Campaña o Lead no encontrado");

                return Ok("Lead asociado a la campaña exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar lead {LeadId} a campaña {CampaignId}", leadId, campaignId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Remueve un lead de una campaña
        /// </summary>
        [HttpDelete("campaigns/{campaignId}/leads/{leadId}")]
        public async Task<ActionResult> RemoveLeadFromCampaign(Guid campaignId, Guid leadId)
        {
            try
            {
                var result = await _crmService.RemoveLeadFromCampaignAsync(campaignId, leadId);
                if (!result)
                    return NotFound("Asociación entre campaña y lead no encontrada");

                return Ok("Lead removido de la campaña exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover lead {LeadId} de campaña {CampaignId}", leadId, campaignId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion

        #region Dashboard/Analytics Endpoints

        /// <summary>
        /// Obtiene métricas generales del CRM para una empresa
        /// </summary>
        [HttpGet("dashboard/{companyId}")]
        public async Task<ActionResult<object>> GetCRMDashboard(Guid companyId)
        {
            try
            {
                var dashboard = await _crmService.GetCRMDashboardAsync(companyId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dashboard CRM para empresa {CompanyId}", companyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion
    }
}