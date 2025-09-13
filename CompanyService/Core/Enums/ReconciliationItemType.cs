namespace CompanyService.Core.Enums
{
    public enum ReconciliationItemType
    {
        BankTransaction = 0,
        OutstandingDeposit = 1,
        OutstandingCheck = 2,
        BankFee = 3,
        Interest = 4,
        Adjustment = 5,
        Error = 6
    }
}