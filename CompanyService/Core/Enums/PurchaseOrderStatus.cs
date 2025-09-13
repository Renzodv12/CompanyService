namespace CompanyService.Core.Enums
{
    public enum PurchaseOrderStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Approved = 2,
        Rejected = 3,
        Sent = 4,
        Acknowledged = 5,
        PartiallyReceived = 6,
        FullyReceived = 7,
        Invoiced = 8,
        Completed = 9,
        Cancelled = 10,
        OnHold = 11
    }
}