using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuotationController : ControllerBase
    {
        private readonly IProcurementService _procurementService;

        public QuotationController(IProcurementService procurementService)
        {
            _procurementService = procurementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuotationResponse>>> GetQuotations(
            [FromQuery] QuotationFilterRequest filter)
        {
            try
            {
                var result = await _procurementService.GetQuotationsAsync(filter.CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuotationResponse>> GetQuotation(Guid id)
        {
            try
            {
                var result = await _procurementService.GetQuotationByIdAsync(id);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuotationResponse>> CreateQuotation(
            [FromBody] CreateQuotationRequest request)
        {
            try
            {
                // Convert DTO to entity - this needs proper mapping
                var quotation = new Core.Entities.Quotation
                {
                    CompanyId = new Guid(), // TODO: Fix CompanyId mapping from int to Guid
                    SupplierId = new Guid(), // TODO: Fix SupplierId mapping from int to Guid
                    QuotationNumber = request.QuotationNumber ?? string.Empty,
                    // Add other required properties
                };
                var result = await _procurementService.CreateQuotationAsync(quotation);
                return CreatedAtAction(nameof(GetQuotation), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuotationResponse>> UpdateQuotation(
            Guid id, [FromBody] UpdateQuotationRequest request)
        {
            try
            {
                // Convert DTO to entity - this needs proper mapping
                var quotation = new Core.Entities.Quotation
                {
                    Id = id,
                    // Map other properties from request
                };
                var result = await _procurementService.UpdateQuotationAsync(id, quotation);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<QuotationResponse>> UpdateQuotationStatus(
            Guid id, [FromBody] UpdateQuotationStatusRequest request)
        {
            try
            {
                var result = await _procurementService.UpdateQuotationStatusAsync(id, request.Status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuotation(Guid id)
        {
            try
            {
                await _procurementService.DeleteQuotationAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/items")]
        public async Task<ActionResult<QuotationItemResponse>> AddQuotationItem(
            Guid id, [FromBody] CreateQuotationItemRequest request)
        {
            try
            {
                // Convert DTO to entity
                var item = new Core.Entities.QuotationItem
                {
                    QuotationId = id,
                    ProductId = new Guid(), // TODO: Fix ProductId mapping from int to Guid
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    // Map other properties
                };
                var result = await _procurementService.AddQuotationItemAsync(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/items/{itemId}")]
        public async Task<ActionResult<QuotationItemResponse>> UpdateQuotationItem(
            Guid id, Guid itemId, [FromBody] UpdateQuotationItemRequest request)
        {
            try
            {
                // Convert DTO to entity
                var item = new Core.Entities.QuotationItem
                {
                    Id = itemId,
                    QuotationId = id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    // Map other properties
                };
                var result = await _procurementService.UpdateQuotationItemAsync(item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<ActionResult> RemoveQuotationItem(Guid id, Guid itemId)
        {
            try
            {
                await _procurementService.RemoveQuotationItemAsync(itemId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/convert-to-purchase-order")]
        public async Task<ActionResult<PurchaseOrderResponse>> ConvertToPurchaseOrder(
            Guid id, [FromBody] ConvertQuotationToPurchaseOrderRequest request)
        {
            try
            {
                // Assuming the service method takes the quotation ID and user ID
                var result = await _procurementService.ConvertQuotationToPurchaseOrderAsync(id, Guid.NewGuid()); // TODO: Get actual user ID
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("compare")]
        public async Task<ActionResult<QuotationComparisonResponse>> CompareQuotations(
            [FromBody] QuotationComparisonRequest request)
        {
            try
            {
                var result = await _procurementService.CompareQuotationsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("reports")]
        public async Task<ActionResult<QuotationReportResponse>> GetQuotationReport(
            [FromQuery] QuotationReportRequest request)
        {
            try
            {
                var result = await _procurementService.GetQuotationReportAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}