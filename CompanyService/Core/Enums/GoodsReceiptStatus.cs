namespace CompanyService.Core.Enums
{
    public enum GoodsReceiptStatus
    {
        Draft = 0,
        Received = 1,
        UnderInspection = 2,
        PartiallyAccepted = 3,
        FullyAccepted = 4,
        Rejected = 5,
        Completed = 6,
        Cancelled = 7
    }
}