using CompanyService.Core.DTOs;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CompanyService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReorderPointController : ControllerBase
    {
        private readonly IReorderPointService _reorderPointService;

        public ReorderPointController(IReorderPointService reorderPointService)
        {
            _reorderPointService = reorderPointService;
        }

        [HttpGet("triggered")]
        public async Task<ActionResult<IEnumerable<ReorderPointResponseDto>>> GetTriggeredReorderPoints()
        {
            var companyId = GetCompanyId();
            var triggeredPoints = await _reorderPointService.GetTriggeredReorderPointsAsync(companyId);
            return Ok(triggeredPoints);
        }

        [HttpPost("{id}/trigger")]
        public async Task<IActionResult> TriggerReorderPoint(Guid id, [FromBody] decimal currentQuantity)
        {
            var success = await _reorderPointService.TriggerReorderPointAsync(id, currentQuantity);
            
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPost("{id}/mark-ordered")]
        public async Task<IActionResult> MarkAsOrdered(Guid id)
        {
            var success = await _reorderPointService.MarkAsOrderedAsync(id);
            
            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetReorderStats()
        {
            var companyId = GetCompanyId();
            var stats = await _reorderPointService.GetReorderStatsAsync(companyId);
            return Ok(stats);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReorderPointDto>>> GetReorderPoints()
        {
            var companyId = GetCompanyId();
            var reorderPoints = await _reorderPointService.GetReorderPointsByCompanyAsync(companyId);
            return Ok(reorderPoints);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReorderPointDto>> GetReorderPoint(Guid id)
        {
            var companyId = GetCompanyId();
            var reorderPoint = await _reorderPointService.GetReorderPointByIdAsync(id);
            
            if (reorderPoint == null)
                return NotFound();

            return Ok(reorderPoint);
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<ReorderPointDto>>> GetReorderPointsByWarehouse(Guid warehouseId)
        {
            var companyId = GetCompanyId();
            var reorderPoints = await _reorderPointService.GetReorderPointsByWarehouseAsync(warehouseId);
            return Ok(reorderPoints);
        }

        [HttpGet("alerts")]
        public async Task<ActionResult<IEnumerable<ReorderAlertDto>>> GetReorderPointAlerts()
        {
            var companyId = GetCompanyId();
            var alerts = await _reorderPointService.GetActiveReorderAlertsAsync(companyId);
            return Ok(alerts);
        }

        [HttpPost]
        public async Task<ActionResult<ReorderPointResponseDto>> CreateReorderPoint([FromBody] CreateReorderPointDto createReorderPointDto)
        {
            var reorderPoint = await _reorderPointService.CreateReorderPointAsync(createReorderPointDto);
            return CreatedAtAction(nameof(GetReorderPoint), new { id = reorderPoint.Id }, reorderPoint);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReorderPointResponseDto>> UpdateReorderPoint(Guid id, [FromBody] UpdateReorderPointDto updateReorderPointDto)
        {
            var reorderPoint = await _reorderPointService.UpdateReorderPointAsync(id, updateReorderPointDto);
            
            if (reorderPoint == null)
                return NotFound();

            return Ok(reorderPoint);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReorderPoint(Guid id)
        {
            var success = await _reorderPointService.DeleteReorderPointAsync(id);
            
            if (!success)
                return NotFound();

            return NoContent();
        }



        private Guid GetCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out Guid companyId))
            {
                throw new UnauthorizedAccessException("Company ID not found in token");
            }
            return companyId;
        }
    }
}