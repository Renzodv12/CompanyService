using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IBankReconciliationService
    {
        // Bank Account Management
        Task<BankAccountResponseDto> CreateBankAccountAsync(CreateBankAccountDto dto);
        Task<BankAccountResponseDto> GetBankAccountByIdAsync(Guid id);
        Task<IEnumerable<BankAccountResponseDto>> GetBankAccountsByCompanyAsync(Guid companyId);
        Task<BankAccountResponseDto> UpdateBankAccountAsync(Guid id, CreateBankAccountDto dto);
        Task<bool> DeleteBankAccountAsync(Guid id);
        Task<bool> ActivateDeactivateBankAccountAsync(Guid id, bool isActive);
        
        // Bank Transaction Management
        Task<BankTransactionResponseDto> CreateBankTransactionAsync(CreateBankTransactionDto dto);
        Task<BankTransactionResponseDto> GetBankTransactionByIdAsync(Guid id);
        Task<IEnumerable<BankTransactionResponseDto>> GetBankTransactionsByAccountAsync(Guid bankAccountId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<BankTransactionResponseDto>> GetUnreconciledTransactionsAsync(Guid bankAccountId);
        Task<BankTransactionResponseDto> UpdateBankTransactionAsync(Guid id, CreateBankTransactionDto dto);
        Task<bool> DeleteBankTransactionAsync(Guid id);
        
        // Bank Reconciliation
        Task<bool> ReconcileTransactionAsync(Guid transactionId, DateTime reconciledDate);
        Task<bool> ReconcileMultipleTransactionsAsync(List<Guid> transactionIds, DateTime reconciledDate);
        Task<bool> UnreconcileTransactionAsync(Guid transactionId);
        Task<object> GetReconciliationReportAsync(Guid bankAccountId, DateTime startDate, DateTime endDate);
        
        // Balance Management
        Task<decimal> GetCurrentBalanceAsync(Guid bankAccountId);
        Task<decimal> GetReconciledBalanceAsync(Guid bankAccountId);
        Task<decimal> GetUnreconciledBalanceAsync(Guid bankAccountId);
        Task<object> GetBalanceHistoryAsync(Guid bankAccountId, DateTime startDate, DateTime endDate);
        
        // Bank Statement Import
        Task<IEnumerable<BankTransactionResponseDto>> ImportBankStatementAsync(Guid bankAccountId, Stream fileStream, string fileName);
        Task<object> ValidateBankStatementAsync(Guid bankAccountId, Stream fileStream, string fileName);
    }
}