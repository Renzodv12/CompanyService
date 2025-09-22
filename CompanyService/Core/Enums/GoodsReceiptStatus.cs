namespace CompanyService.Core.Enums
{
    public enum GoodsReceiptStatus
    {
        Draft = 0,
        Pending = 1,
        Received = 2,
        UnderInspection = 3,
        PartiallyAccepted = 4,
        FullyAccepted = 5,
        Rejected = 6,
        Completed = 7,
        Cancelled = 8
    }
}