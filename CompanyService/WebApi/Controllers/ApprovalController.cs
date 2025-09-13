using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IProcurementService _procurementService;

        public ApprovalController(IProcurementService procurementService)
        {
            _procurementService = procurementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApprovalResponse>>> GetApprovals(
            [FromQuery] ApprovalFilterRequest filter)
        {
            try
            {
                var result = await _procurementService.GetApprovalsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApprovalResponse>> GetApproval(Guid id)
        {
            try
            {
                var result = await _procurementService.GetApprovalByIdAsync(id);
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
        public async Task<ActionResult<ApprovalResponse>> CreateApproval(
            [FromBody] CreateApprovalRequest request)
        {
            try
            {
                // Convert DTO to entity
                var approval = new Core.Entities.Approval
                {
                    CompanyId = new Guid(request.CompanyId.ToString().PadLeft(32, '0')), // TODO: Implement proper CompanyId mapping
                    DocumentId = new Guid(request.DocumentId.ToString().PadLeft(32, '0')), // TODO: Implement proper DocumentId mapping
                    DocumentType = request.DocumentType,
                    DocumentNumber = $"APR-{DateTime.UtcNow:yyyyMMdd}-{request.DocumentId}", // Generate document number
                    RequestedBy = request.RequestedByUserId.HasValue ? new Guid(request.RequestedByUserId.Value.ToString().PadLeft(32, '0')) : new Guid(request.UserId.ToString().PadLeft(32, '0')), // TODO: Implement proper UserId mapping
                    ApprovalLevelId = new Guid(request.ApprovalLevelId.ToString().PadLeft(32, '0')), // TODO: Implement proper ApprovalLevelId mapping
                    ApproverId = new Guid(request.UserId.ToString().PadLeft(32, '0')), // TODO: Implement proper ApproverId mapping
                    ApprovalOrder = 1, // Default order
                    RequestedDate = DateTime.UtcNow,
                    Status = Core.Enums.ApprovalStatus.Pending,
                    DocumentAmount = request.DocumentAmount,
                    Comments = request.Comments,
                    DueDate = request.DueDate,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                var result = await _procurementService.CreateApprovalAsync(approval);
                return CreatedAtAction(nameof(GetApproval), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApprovalResponse>> UpdateApproval(
            Guid id, [FromBody] UpdateApprovalRequest request)
        {
            try
            {
                var result = await _procurementService.UpdateApprovalAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/process")]
        public async Task<ActionResult<ApprovalResponse>> ProcessApproval(
            Guid id, [FromBody] ProcessApprovalRequest request)
        {
            try
            {
                // Map ApprovalAction to ApprovalStatus
                var status = request.Action switch
                {
                    Core.Enums.ApprovalAction.Approve => Core.Enums.ApprovalStatus.Approved,
                    Core.Enums.ApprovalAction.Reject => Core.Enums.ApprovalStatus.Rejected,
                    Core.Enums.ApprovalAction.Delegate => Core.Enums.ApprovalStatus.Delegated,
                    Core.Enums.ApprovalAction.RequestMoreInfo => Core.Enums.ApprovalStatus.InProgress,
                    _ => Core.Enums.ApprovalStatus.Pending
                };
                
                var result = await _procurementService.ProcessApprovalAsync(id, status, request.Comments);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteApproval(Guid id)
        {
            try
            {
                await _procurementService.DeleteApprovalAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ApprovalResponse>>> GetPendingApprovals(
            [FromQuery] Guid userId)
        {
            try
            {
                var result = await _procurementService.GetPendingApprovalsForUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("reports")]
        public async Task<ActionResult<ApprovalReportResponse>> GetApprovalReport(
            [FromQuery] ApprovalReportRequest request)
        {
            try
            {
                var result = await _procurementService.GetApprovalReportAsync(new Guid(), request.StartDate, request.EndDate); // TODO: Fix CompanyId mapping from int to Guid
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Approval Level Management
        [HttpGet("levels")]
        public async Task<ActionResult<IEnumerable<ApprovalLevelResponse>>> GetApprovalLevels(
            [FromQuery] Guid companyId)
        {
            try
            {
                var result = await _procurementService.GetApprovalLevelsAsync(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("levels")]
        public async Task<ActionResult<ApprovalLevelResponse>> CreateApprovalLevel(
            [FromBody] CreateApprovalLevelRequest request)
        {
            try
            {
                var result = await _procurementService.CreateApprovalLevelAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("levels/{id}")]
        public async Task<ActionResult<ApprovalLevelResponse>> UpdateApprovalLevel(
            Guid id, [FromBody] UpdateApprovalLevelRequest request)
        {
            try
            {
                var result = await _procurementService.UpdateApprovalLevelAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("levels/{id}")]
        public async Task<ActionResult> DeleteApprovalLevel(Guid id)
        {
            try
            {
                await _procurementService.DeleteApprovalLevelAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Approval Level User Management
        [HttpPost("levels/{levelId}/users")]
        public async Task<ActionResult<ApprovalLevelUserResponse>> AddUserToApprovalLevel(
            Guid levelId, [FromBody] CreateApprovalLevelUserRequest request)
        {
            try
            {
                var result = await _procurementService.AddUserToApprovalLevelAsync(levelId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("levels/{levelId}/users/{userId}")]
        public async Task<ActionResult<ApprovalLevelUserResponse>> UpdateApprovalLevelUser(
            Guid levelId, Guid userId, [FromBody] UpdateApprovalLevelUserRequest request)
        {
            try
            {
                var result = await _procurementService.UpdateApprovalLevelUserAsync(levelId, userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("levels/{levelId}/users/{userId}")]
        public async Task<ActionResult> RemoveUserFromApprovalLevel(Guid levelId, Guid userId)
        {
            try
            {
                var result = await _procurementService.RemoveUserFromApprovalLevelAsync(levelId, userId);
                if (result)
                {
                    return Ok(new { message = "User removed from approval level successfully" });
                }
                return NotFound(new { message = "Approval level user not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}