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
    public class PhysicalInventoryController : ControllerBase
    {
        private readonly IPhysicalInventoryService _physicalInventoryService;

        public PhysicalInventoryController(IPhysicalInventoryService physicalInventoryService)
        {
            _physicalInventoryService = physicalInventoryService;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhysicalInventoryResponseDto>>> GetPhysicalInventories()
        {
            try
            {
                var companyId = GetCompanyId();
                var inventories = await _physicalInventoryService.GetPhysicalInventoriesByCompanyAsync(companyId);
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhysicalInventoryResponseDto>> GetPhysicalInventory(Guid id)
        {
            try
            {
                var inventory = await _physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                
                if (inventory == null)
                    return NotFound();

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/items")]
        public async Task<ActionResult<IEnumerable<PhysicalInventoryItemResponseDto>>> GetInventoryItems(Guid id)
        {
            try
            {
                var items = await _physicalInventoryService.GetInventoryItemsAsync(id);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<PhysicalInventoryResponseDto>>> GetPhysicalInventoriesByWarehouse(Guid warehouseId)
        {
            try
            {
                var inventories = await _physicalInventoryService.GetPhysicalInventoriesByWarehouseAsync(warehouseId);
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PhysicalInventoryResponseDto>> CreatePhysicalInventory([FromBody] CreatePhysicalInventoryDto createDto)
        {
            try
            {
                var inventory = await _physicalInventoryService.CreatePhysicalInventoryAsync(createDto);
                return CreatedAtAction(nameof(GetPhysicalInventory), new { id = inventory.Id }, inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PhysicalInventoryResponseDto>> UpdatePhysicalInventory(Guid id, [FromBody] UpdatePhysicalInventoryDto updateDto)
        {
            try
            {
                var inventory = await _physicalInventoryService.UpdatePhysicalInventoryAsync(id, updateDto);
                
                if (inventory == null)
                    return NotFound();

                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult> StartPhysicalInventory(Guid id)
        {
            try
            {
                var success = await _physicalInventoryService.StartPhysicalInventoryAsync(id);
                
                if (!success)
                    return NotFound();

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompletePhysicalInventory(Guid id)
        {
            try
            {
                var success = await _physicalInventoryService.CompletePhysicalInventoryAsync(id);
                
                if (!success)
                    return NotFound();

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelPhysicalInventory(Guid id)
        {
            try
            {
                var success = await _physicalInventoryService.CancelPhysicalInventoryAsync(id);
                
                if (!success)
                    return NotFound();

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/items/{itemId}")]
        public async Task<ActionResult<PhysicalInventoryItemResponseDto>> UpdatePhysicalInventoryItem(Guid id, Guid itemId, [FromBody] UpdatePhysicalInventoryItemDto updateItemDto)
        {
            try
            {
                var item = await _physicalInventoryService.UpdateInventoryItemAsync(id, itemId, updateItemDto);
                
                if (item == null)
                    return NotFound();

                return Ok(item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/variance-report")]
        public async Task<ActionResult<object>> GetInventoryVarianceReport(Guid id)
        {
            try
            {
                var report = await _physicalInventoryService.GetInventoryVarianceReportAsync(id);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}