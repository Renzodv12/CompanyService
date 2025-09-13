using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompanyService.Core.Enums;

namespace CompanyService.Core.DTOs.Procurement
{
    public class CreateApprovalRequest
    {
        public int CompanyId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public int ApprovalLevelId { get; set; }
        public int UserId { get; set; }
        public decimal? DocumentAmount { get; set; }
        public string? Comments { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string? Priority { get; set; }
        public string? Reason { get; set; }
        public int? RequestedByUserId { get; set; }
        public decimal? Amount { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
    }

    public class UpdateApprovalRequest
    {
        public ApprovalStatus Status { get; set; }
        public string? Comments { get; set; }
        public string? RejectionReason { get; set; }
        public string? Notes { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string? Reason { get; set; }
        public string? Priority { get; set; }
    }

    public class ProcessApprovalRequest
    {
        public ApprovalAction Action { get; set; }
        public string Comments { get; set; } = string.Empty;
        public int? DelegateToUserId { get; set; }
    }

    public class ApprovalResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public string? DocumentNumber { get; set; }
        public int ApprovalLevelId { get; set; }
        public string ApprovalLevelName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ApprovalStatus Status { get; set; }
        public decimal? DocumentAmount { get; set; }
        public string? Comments { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ApprovalSummaryResponse
    {
        public int Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public string? DocumentNumber { get; set; }
        public string ApprovalLevelName { get; set; } = string.Empty;
        public int Level { get; set; }
        public ApprovalStatus Status { get; set; }
        public decimal? DocumentAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class CreateApprovalLevelRequest
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int Level { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool RequiresAllApprovers { get; set; }
        public int? AutoApprovalDays { get; set; }
        public bool IsActive { get; set; } = true;
        public int RequiredApprovals { get; set; }
        public List<int> UserIds { get; set; } = new();
    }

    public class UpdateApprovalLevelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool RequiresAllApprovers { get; set; }
        public int RequiredApprovals { get; set; }
        public int? AutoApprovalDays { get; set; }
        public bool IsActive { get; set; }
        public List<int> UserIds { get; set; } = new();
    }

    public class ApprovalLevelResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public int Level { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool RequiresAllApprovers { get; set; }
        public int? AutoApprovalDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ApprovalLevelUserResponse> Users { get; set; } = new();
    }

    public class ApprovalLevelUserResponse
    {
        public int Id { get; set; }
        public int ApprovalLevelId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? DelegatedToUserId { get; set; }
        public string? DelegatedToUserName { get; set; }
        public DateTime? DelegationStartDate { get; set; }
        public DateTime? DelegationEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateApprovalLevelUserRequest
    {
        public int ApprovalLevelId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool CanDelegate { get; set; }
        public int? MaxDelegationLevel { get; set; }
    }

    public class UpdateApprovalLevelUserRequest
    {
        public bool IsActive { get; set; }
        public int? DelegatedToUserId { get; set; }
        public DateTime? DelegationStartDate { get; set; }
        public DateTime? DelegationEndDate { get; set; }
        public bool CanDelegate { get; set; }
        public int? MaxDelegationLevel { get; set; }
    }

    public class ApprovalWorkflowRequest
    {
        public string DocumentType { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public decimal DocumentAmount { get; set; }
        public string? Comments { get; set; }
    }

    public class ApprovalWorkflowResponse
    {
        public string DocumentType { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public string? DocumentNumber { get; set; }
        public decimal DocumentAmount { get; set; }
        public List<ApprovalStepResponse> Steps { get; set; } = new();
        public ApprovalStatus OverallStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class ApprovalStepResponse
    {
        public int ApprovalId { get; set; }
        public int Level { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public List<ApprovalUserResponse> Approvers { get; set; } = new();
        public ApprovalStatus Status { get; set; }
        public bool RequiresAllApprovers { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class ApprovalUserResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ApprovalStatus Status { get; set; }
        public string? Comments { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsDelegated { get; set; }
        public string? DelegatedFromUserName { get; set; }
    }

    public class ApprovalFilterRequest
    {
        public string? DocumentType { get; set; }
        public ApprovalStatus? Status { get; set; }
        public int? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IncludeOverdue { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class BulkApprovalRequest
    {
        public List<int> ApprovalIds { get; set; } = new();
        public ApprovalStatus Status { get; set; }
        public string? Comments { get; set; }
    }

    public class DelegateApprovalRequest
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int>? ApprovalLevelIds { get; set; }
    }

    public class ApprovalReportRequest
    {
        public int CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? DocumentType { get; set; }
        public string? EntityType { get; set; }
        public ApprovalStatus? Status { get; set; }
        public int? UserId { get; set; }
        public string? ReportType { get; set; }
    }

    public class ApprovalReportResponse
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public int TotalApprovals { get; set; }
        public int PendingApprovals { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal AverageProcessingTime { get; set; }
        public List<ApprovalSummary> ApprovalSummaries { get; set; } = new();
        public List<UserApprovalSummary> UserSummaries { get; set; } = new();
    }

    public class ApprovalSummary
    {
        public int Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public string? ProcessorName { get; set; }
    }

    public class UserApprovalSummary
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalProcessed { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal AverageProcessingTime { get; set; }
    }
}