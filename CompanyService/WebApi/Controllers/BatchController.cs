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
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;

        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
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
        public async Task<ActionResult<IEnumerable<BatchResponseDto>>> GetBatches()
        {
            try
            {
                var companyId = GetCompanyId();
                var batches = await _batchService.GetBatchesByCompanyAsync(companyId);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BatchResponseDto>> GetBatch(Guid id)
        {
            try
            {
                var batch = await _batchService.GetBatchByIdAsync(id);
                
                if (batch == null)
                {
                    return NotFound(new { message = "Lote no encontrado" });
                }

                return Ok(batch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<BatchResponseDto>>> GetBatchesByProduct(Guid productId)
        {
            try
            {
                var batches = await _batchService.GetBatchesByProductAsync(productId);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<BatchResponseDto>>> GetBatchesByWarehouse(Guid warehouseId)
        {
            try
            {
                var batches = await _batchService.GetBatchesByWarehouseAsync(warehouseId);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BatchResponseDto>> CreateBatch([FromBody] CreateBatchDto createBatchDto)
        {
            try
            {
                var batch = await _batchService.CreateBatchAsync(createBatchDto);
                return CreatedAtAction(nameof(GetBatch), new { id = batch.Id }, batch);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BatchResponseDto>> UpdateBatch(Guid id, [FromBody] UpdateBatchDto updateBatchDto)
        {
            try
            {
                var batch = await _batchService.UpdateBatchAsync(id, updateBatchDto);
                
                if (batch == null)
                {
                    return NotFound(new { message = "Lote no encontrado" });
                }

                return Ok(batch);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost("move")]
        public async Task<ActionResult> MoveBatchQuantity([FromBody] BatchMovementDto movementDto)
        {
            try
            {
                var result = await _batchService.MoveBatchQuantityAsync(movementDto);
                if (result)
                {
                    return Ok();
                }
                return BadRequest("Failed to move batch quantity");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("expired")]
        public async Task<ActionResult<IEnumerable<BatchResponseDto>>> GetExpiredBatches()
        {
            try
            {
                var companyId = GetCompanyId();
                var batches = await _batchService.GetExpiredBatchesAsync(companyId);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("near-expiration")]
        public async Task<ActionResult<IEnumerable<BatchResponseDto>>> GetNearExpirationBatches([FromQuery] int days = 30)
        {
            try
            {
                var companyId = GetCompanyId();
                var batches = await _batchService.GetNearExpirationBatchesAsync(companyId, days);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("availability/{productId}/{warehouseId}")]
        public async Task<ActionResult<decimal>> GetAvailableQuantity(Guid productId, Guid warehouseId)
        {
            try
            {
                var quantity = await _batchService.GetAvailableQuantityAsync(productId, warehouseId);
                return Ok(new { availableQuantity = quantity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}