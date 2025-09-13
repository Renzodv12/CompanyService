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
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
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
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetWarehouses()
        {
            try
            {
                var companyId = GetCompanyId();
                var warehouses = await _warehouseService.GetWarehousesByCompanyAsync(companyId);
                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseResponseDto>> GetWarehouse(Guid id)
        {
            try
            {
                var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
                
                if (warehouse == null)
                {
                    return NotFound(new { message = "Almacén no encontrado" });
                }

                return Ok(warehouse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<WarehouseResponseDto>> CreateWarehouse([FromBody] CreateWarehouseDto createWarehouseDto)
        {
            try
            {
                var companyId = GetCompanyId();
                createWarehouseDto.CompanyId = companyId;
                var warehouse = await _warehouseService.CreateWarehouseAsync(createWarehouseDto);
                return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
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
        public async Task<ActionResult<WarehouseResponseDto>> UpdateWarehouse(Guid id, [FromBody] UpdateWarehouseDto updateWarehouseDto)
        {
            try
            {
                var warehouse = await _warehouseService.UpdateWarehouseAsync(id, updateWarehouseDto);
                
                if (warehouse == null)
                {
                    return NotFound(new { message = "Almacén no encontrado" });
                }

                return Ok(warehouse);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWarehouse(Guid id)
        {
            try
            {
                var result = await _warehouseService.DeleteWarehouseAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "Almacén no encontrado" });
                }

                return NoContent();
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

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetActiveWarehouses()
        {
            try
            {
                var companyId = GetCompanyId();
                var warehouses = await _warehouseService.GetActiveWarehousesByCompanyAsync(companyId);
                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}