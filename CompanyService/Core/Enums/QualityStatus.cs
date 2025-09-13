namespace CompanyService.Core.Enums
{
    public enum QualityStatus
    {
        NotInspected = 0,
        Pending = 1,
        InProgress = 2,
        Passed = 3,
        Failed = 4,
        PartiallyAccepted = 5,
        Rejected = 6,
        RequiresRework = 7,
        OnHold = 8
    }
}