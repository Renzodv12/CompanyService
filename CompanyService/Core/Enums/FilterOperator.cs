namespace CompanyService.Core.Enums
{
    public enum FilterOperator
    {
        Equals = 0,
        NotEquals = 1,
        Contains = 2,
        NotContains = 3,
        StartsWith = 4,
        EndsWith = 5,
        GreaterThan = 6,
        GreaterThanOrEqual = 7,
        LessThan = 8,
        LessThanOrEqual = 9,
        Between = 10,
        In = 11,
        NotIn = 12,
        IsNull = 13,
        IsNotNull = 14
    }
}