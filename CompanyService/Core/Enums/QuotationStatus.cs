namespace CompanyService.Core.Enums
{
    public enum QuotationStatus
    {
        Draft = 0,
        Requested = 1,
        Received = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        Accepted = 6,
        ConvertedToPO = 7,
        Expired = 8,
        Cancelled = 9
    }
}