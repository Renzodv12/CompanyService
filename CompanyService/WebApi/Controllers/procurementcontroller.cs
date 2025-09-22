using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CompanyService.WebApi.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId:guid}/procurement")]
    [Authorize]
    public class ProcurementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IProcurementService _procurementService;
        private readonly ILogger<ProcurementController> _logger;

        public ProcurementController(
            IMediator mediator,
            IProcurementService procurementService,
            ILogger<ProcurementController> logger)
        {
            _mediator = mediator;
            _procurementService = procurementService;
            _logger = logger;
        }

        [HttpGet("purchase-orders")]
        public async Task<ActionResult<IEnumerable<PurchaseOrderResponse>>> GetPurchaseOrders(
            Guid companyId,
            [FromQuery] PurchaseOrderFilterRequest? filter = null)
        {
            try
            {
                var query = new GetPurchaseOrdersQuery
                {
                    CompanyId = companyId,
                    Filter = filter
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase orders for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("purchase-orders/{id:guid}")]
        public async Task<ActionResult<PurchaseOrderResponse>> GetPurchaseOrderById(
            Guid companyId,
            Guid id)
        {
            try
            {
                var query = new GetPurchaseOrderByIdQuery
                {
                    CompanyId = companyId,
                    Id = id
                };

                var result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase order {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("purchase-orders")]
        public async Task<ActionResult<PurchaseOrderResponse>> CreatePurchaseOrder(
            Guid companyId,
            [FromBody] CreatePurchaseOrderRequest request)
        {
            try
            {
                var command = new CreatePurchaseOrderCommand
                {
                    CompanyId = companyId,
                    Request = request
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetPurchaseOrderById),
                    new { companyId, id = result.Id },
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("purchase-orders/{id:guid}")]
        public async Task<ActionResult<PurchaseOrderResponse>> UpdatePurchaseOrder(
            Guid companyId,
            Guid id,
            [FromBody] UpdatePurchaseOrderRequest request)
        {
            try
            {
                var command = new UpdatePurchaseOrderCommand
                {
                    CompanyId = companyId,
                    Id = id,
                    Request = request
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("purchase-orders/{id:guid}")]
        public async Task<ActionResult> DeletePurchaseOrder(
            Guid companyId,
            Guid id)
        {
            try
            {
                var command = new DeletePurchaseOrderCommand
                {
                    CompanyId = companyId,
                    Id = id
                };

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase order {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("quotations")]
        public async Task<ActionResult<IEnumerable<QuotationResponse>>> GetQuotations(
            Guid companyId,
            [FromQuery] QuotationFilterRequest? filter = null)
        {
            try
            {
                var query = new GetQuotationsQuery
                {
                    CompanyId = companyId,
                    Filter = filter
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quotations for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("quotations/{id:guid}")]
        public async Task<ActionResult<QuotationResponse>> GetQuotationById(
            Guid companyId,
            Guid id)
        {
            try
            {
                var query = new GetQuotationByIdQuery
                {
                    CompanyId = companyId,
                    Id = id
                };

                var result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quotation {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("quotations")]
        public async Task<ActionResult<QuotationResponse>> CreateQuotation(
            Guid companyId,
            [FromBody] CreateQuotationRequest request)
        {
            try
            {
                var command = new CreateQuotationCommand
                {
                    CompanyId = companyId,
                    Request = request
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetQuotationById),
                    new { companyId, id = result.Id },
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quotation for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("quotations/{id:guid}")]
        public async Task<ActionResult<QuotationResponse>> UpdateQuotation(
            Guid companyId,
            Guid id,
            [FromBody] UpdateQuotationRequest request)
        {
            try
            {
                var command = new UpdateQuotationCommand
                {
                    CompanyId = companyId,
                    Id = id,
                    Request = request
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quotation {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("quotations/{id:guid}")]
        public async Task<ActionResult> DeleteQuotation(
            Guid companyId,
            Guid id)
        {
            try
            {
                var command = new DeleteQuotationCommand
                {
                    CompanyId = companyId,
                    Id = id
                };

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quotation {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("approvals")]
        public async Task<ActionResult<IEnumerable<ApprovalResponse>>> GetApprovals(
            Guid companyId,
            [FromQuery] ApprovalFilterRequest? filter = null)
        {
            try
            {
                var query = new GetApprovalsQuery
                {
                    CompanyId = companyId,
                    Filter = filter
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approvals for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("approvals/pending")]
        public async Task<ActionResult<IEnumerable<ApprovalResponse>>> GetPendingApprovals(
            Guid companyId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                
                var query = new GetPendingApprovalsQuery
                {
                    CompanyId = companyId,
                    UserId = userId
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending approvals for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        // Line 278 - Fixed the void assignment error
        [HttpPost("approvals/{id:guid}/process")]
        public async Task<ActionResult> ProcessApproval(
            Guid companyId,
            Guid id,
            [FromBody] ProcessApprovalRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                
                var command = new ProcessApprovalCommand
                {
                    CompanyId = companyId,
                    Id = id,
                    UserId = userId,
                    Action = request.Action,
                    Comments = request.Comments,
                    DelegateToUserId = request.DelegateToUserId
                };

                // Fixed: Don't assign void result to variable
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing approval {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("approvals/{id:guid}/approve")]
        public async Task<ActionResult> ApproveApproval(
            Guid companyId,
            Guid id,
            [FromBody] ProcessApprovalRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                
                var command = new ProcessApprovalCommand
                {
                    CompanyId = companyId,
                    Id = id,
                    UserId = userId,
                    Action = ApprovalAction.Approve,
                    Comments = request.Comments
                };

                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving approval {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("approvals/{id:guid}/reject")]
        public async Task<ActionResult> RejectApproval(
            Guid companyId,
            Guid id,
            [FromBody] ProcessApprovalRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                
                var command = new ProcessApprovalCommand
                {
                    CompanyId = companyId,
                    Id = id,
                    UserId = userId,
                    Action = ApprovalAction.Reject,
                    Comments = request.Comments
                };

                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting approval {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("approval-levels")]
        public async Task<ActionResult<IEnumerable<ApprovalLevelResponse>>> GetApprovalLevels(
            Guid companyId)
        {
            try
            {
                var query = new GetApprovalLevelsQuery
                {
                    CompanyId = companyId
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval levels for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("approval-levels")]
        public async Task<ActionResult<ApprovalLevelResponse>> CreateApprovalLevel(
            Guid companyId,
            [FromBody] CreateApprovalLevelRequest request)
        {
            try
            {
                var command = new CreateApprovalLevelCommand
                {
                    CompanyId = companyId,
                    Name = request.Name,
                    Description = request.Description,
                    Level = request.Level,
                    MinAmount = request.MinAmount,
                    MaxAmount = request.MaxAmount,
                    RequiresAllApprovers = request.RequiresAllApprovers,
                    IsActive = request.IsActive,
                    UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException())
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetApprovalLevels),
                    new { companyId },
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval level for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("approval-levels/{id:guid}")]
        public async Task<ActionResult<ApprovalLevelResponse>> UpdateApprovalLevel(
            Guid companyId,
            Guid id,
            [FromBody] UpdateApprovalLevelRequest request)
        {
            try
            {
                var command = new UpdateApprovalLevelCommand
                {
                    CompanyId = companyId,
                    Id = id,
                    Request = request
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval level {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("approval-levels/{id:guid}")]
        public async Task<ActionResult> DeleteApprovalLevel(
            Guid companyId,
            Guid id)
        {
            try
            {
                var command = new DeleteApprovalLevelCommand
                {
                    CompanyId = companyId,
                    Id = id
                };

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval level {Id} for company {CompanyId}", id, companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        // Line 557 - Fixed ProcessApprovalRequest.Status property issue
        [HttpPost("approvals/bulk-process")]
        public async Task<ActionResult> BulkProcessApprovals(
            Guid companyId,
            [FromBody] BulkProcessApprovalsRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                
                var command = new BulkProcessApprovalsCommand
                {
                    CompanyId = companyId,
                    Request = request
                };

                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk processing approvals for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        // Line 575 - Fixed DelegateApprovalRequest properties
        [HttpPost("approvals/delegate")]
        public async Task<ActionResult> DelegateApproval(
            Guid companyId,
            [FromBody] DelegateApprovalRequest request)
        {
            try
            {
                var command = new DelegateApprovalCommand
                {
                    CompanyId = companyId,
                    FromUserId = request.FromUserId,
                    ToUserId = request.ToUserId, // Fixed: Use ToUserId instead of DelegateToUserId
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ApprovalLevelIds = request.ApprovalLevelIds,
                    Comments = "Delegation request" // Fixed: Use default comment instead of Reason
                };

                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delegating approval for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("goods-receipts")]
        public async Task<ActionResult<IEnumerable<GoodsReceiptResponse>>> GetGoodsReceipts(
            Guid companyId,
            [FromQuery] GoodsReceiptFilterRequest? filter = null)
        {
            try
            {
                var query = new GetGoodsReceiptsQuery
                {
                    CompanyId = companyId,
                    SupplierId = filter?.SupplierId,
                    PurchaseOrderId = filter?.PurchaseOrderId,
                    FromDate = filter?.StartDate,
                    ToDate = filter?.EndDate,
                    Status = filter?.Status?.ToString(),
                    Page = filter?.Page ?? 1,
                    PageSize = filter?.PageSize ?? 10
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting goods receipts for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("goods-receipts")]
        public async Task<ActionResult<GoodsReceiptResponse>> CreateGoodsReceipt(
            Guid companyId,
            [FromBody] CreateGoodsReceiptRequest request)
        {
            try
            {
                var command = new CreateGoodsReceiptCommand
                {
                    CompanyId = companyId,
                    Request = request
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetGoodsReceipts),
                    new { companyId },
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goods receipt for company {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}