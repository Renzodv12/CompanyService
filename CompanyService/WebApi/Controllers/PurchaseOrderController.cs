using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IProcurementService _procurementService;

        public PurchaseOrderController(IProcurementService procurementService)
        {
            _procurementService = procurementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderResponse>>> GetPurchaseOrders(
            [FromQuery] PurchaseOrderFilterRequest filter)
        {
            try
            {
                var result = await _procurementService.GetPurchaseOrdersAsync(filter.CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderResponse>> GetPurchaseOrder(Guid id)
        {
            try
            {
                var result = await _procurementService.GetPurchaseOrderByIdAsync(id);
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
        public async Task<ActionResult<PurchaseOrderResponse>> CreatePurchaseOrder(
            [FromBody] CreatePurchaseOrderRequest request)
        {
            try
            {
                // Convert DTO to entity
                var purchaseOrder = new Core.Entities.PurchaseOrder
                {
                    CompanyId = new Guid(), // TODO: Convert from request.CompanyId (int)
                    SupplierId = new Guid(), // TODO: Convert from request.SupplierId (int)
                    OrderNumber = request.OrderNumber ?? string.Empty,
                    // Map other properties
                };
                var result = await _procurementService.CreatePurchaseOrderAsync(purchaseOrder);
                return CreatedAtAction(nameof(GetPurchaseOrder), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PurchaseOrderResponse>> UpdatePurchaseOrder(
            Guid id, [FromBody] UpdatePurchaseOrderRequest request)
        {
            try
            {
                var result = await _procurementService.UpdatePurchaseOrderAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<PurchaseOrderResponse>> UpdatePurchaseOrderStatus(
            Guid id, [FromBody] UpdatePurchaseOrderStatusRequest request)
        {
            try
            {
                var result = await _procurementService.UpdatePurchaseOrderStatusAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchaseOrder(Guid id)
        {
            try
            {
                await _procurementService.DeletePurchaseOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/items")]
        public async Task<ActionResult<PurchaseOrderItemResponse>> AddPurchaseOrderItem(
            Guid id, [FromBody] CreatePurchaseOrderItemRequest request)
        {
            try
            {
                var result = await _procurementService.AddPurchaseOrderItemAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/items/{itemId}")]
        public async Task<ActionResult<PurchaseOrderItemResponse>> UpdatePurchaseOrderItem(
            Guid id, Guid itemId, [FromBody] UpdatePurchaseOrderItemRequest request)
        {
            try
            {
                var result = await _procurementService.UpdatePurchaseOrderItemAsync(id, itemId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<ActionResult> RemovePurchaseOrderItem(Guid id, Guid itemId)
        {
            try
            {
                await _procurementService.RemovePurchaseOrderItemAsync(id, itemId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<PurchaseOrderResponse>> ApprovePurchaseOrder(Guid id, [FromQuery] Guid approvedBy)
        {
            try
            {
                var result = await _procurementService.ApprovePurchaseOrderAsync(id, approvedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult<PurchaseOrderResponse>> RejectPurchaseOrder(
            Guid id, [FromBody] string reason)
        {
            try
            {
                var result = await _procurementService.RejectPurchaseOrderAsync(id, Guid.NewGuid(), reason); // TODO: Get actual rejectedBy user ID
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("reports")]
        public async Task<ActionResult<PurchaseOrderReportResponse>> GetPurchaseOrderReport(
            [FromQuery] PurchaseOrderReportRequest request)
        {
            try
            {
                var result = await _procurementService.GetPurchaseOrderReportAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}