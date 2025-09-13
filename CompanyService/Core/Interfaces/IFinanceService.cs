using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Interfaces
{
    public interface IFinanceService
    {
        // Accounts Receivable
        Task<AccountsReceivableResponseDto> CreateAccountsReceivableAsync(CreateAccountsReceivableDto dto);
        Task<AccountsReceivableResponseDto> GetAccountsReceivableByIdAsync(Guid id);
        Task<IEnumerable<AccountsReceivableResponseDto>> GetAccountsReceivableByCompanyAsync(Guid companyId);
        Task<IEnumerable<AccountsReceivableResponseDto>> GetOverdueAccountsReceivableAsync(Guid companyId);
        Task<AccountsReceivableResponseDto> UpdateAccountsReceivableAsync(Guid id, CreateAccountsReceivableDto dto);
        Task<bool> DeleteAccountsReceivableAsync(Guid id);
        
        // Accounts Receivable Payments
        Task<AccountsReceivablePaymentResponseDto> CreateAccountsReceivablePaymentAsync(CreateAccountsReceivablePaymentDto dto);
        Task<IEnumerable<AccountsReceivablePaymentResponseDto>> GetPaymentsByAccountsReceivableAsync(Guid accountsReceivableId);
        
        // Accounts Payable
        Task<AccountsPayableResponseDto> CreateAccountsPayableAsync(CreateAccountsPayableDto dto);
        Task<AccountsPayableResponseDto> GetAccountsPayableByIdAsync(Guid id);
        Task<IEnumerable<AccountsPayableResponseDto>> GetAccountsPayableByCompanyAsync(Guid companyId);
        Task<IEnumerable<AccountsPayableResponseDto>> GetOverdueAccountsPayableAsync(Guid companyId);
        Task<AccountsPayableResponseDto> UpdateAccountsPayableAsync(Guid id, CreateAccountsPayableDto dto);
        Task<bool> DeleteAccountsPayableAsync(Guid id);
        
        // Accounts Payable Payments
        Task<AccountsPayablePaymentResponseDto> CreateAccountsPayablePaymentAsync(CreateAccountsPayablePaymentDto dto);
        Task<IEnumerable<AccountsPayablePaymentResponseDto>> GetPaymentsByAccountsPayableAsync(Guid accountsPayableId);
        
        // Budget Management
        Task<BudgetResponseDto> CreateBudgetAsync(CreateBudgetDto dto);
        Task<BudgetResponseDto> GetBudgetByIdAsync(Guid id);
        Task<IEnumerable<BudgetResponseDto>> GetBudgetsByCompanyAsync(Guid companyId, int? year = null, int? month = null);
        Task<BudgetSummaryDto> GetBudgetSummaryAsync(Guid companyId, int year, int? month = null);
        Task<BudgetResponseDto> UpdateBudgetAsync(Guid id, CreateBudgetDto dto);
        Task<bool> DeleteBudgetAsync(Guid id);
        
        // Financial Reports
        Task<decimal> GetTotalAccountsReceivableAsync(Guid companyId);
        Task<decimal> GetTotalAccountsPayableAsync(Guid companyId);
        Task<decimal> GetNetReceivablesAsync(Guid companyId);
        Task<object> GetAgingReportAsync(Guid companyId, bool isReceivable = true);
    }
}