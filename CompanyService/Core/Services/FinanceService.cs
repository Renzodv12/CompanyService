using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountsReceivable> _accountsReceivableRepository;
        private readonly IRepository<AccountsReceivablePayment> _accountsReceivablePaymentRepository;
        private readonly IRepository<AccountsPayable> _accountsPayableRepository;
        private readonly IRepository<AccountsPayablePayment> _accountsPayablePaymentRepository;
        private readonly IRepository<Budget> _budgetRepository;
        private readonly IRepository<BudgetLine> _budgetLineRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Account> _accountRepository;

        public FinanceService(
            IUnitOfWork unitOfWork,
            IRepository<AccountsReceivable> accountsReceivableRepository,
            IRepository<AccountsReceivablePayment> accountsReceivablePaymentRepository,
            IRepository<AccountsPayable> accountsPayableRepository,
            IRepository<AccountsPayablePayment> accountsPayablePaymentRepository,
            IRepository<Budget> budgetRepository,
            IRepository<BudgetLine> budgetLineRepository,
            IRepository<Customer> customerRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Account> accountRepository)
        {
            _unitOfWork = unitOfWork;
            _accountsReceivableRepository = accountsReceivableRepository;
            _accountsReceivablePaymentRepository = accountsReceivablePaymentRepository;
            _accountsPayableRepository = accountsPayableRepository;
            _accountsPayablePaymentRepository = accountsPayablePaymentRepository;
            _budgetRepository = budgetRepository;
            _budgetLineRepository = budgetLineRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _accountRepository = accountRepository;
        }

        #region Accounts Receivable

        public async Task<AccountsReceivableResponseDto> CreateAccountsReceivableAsync(CreateAccountsReceivableDto dto)
        {
            var accountsReceivable = new AccountsReceivable
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = dto.InvoiceNumber,
                CustomerId = dto.CustomerId,
                SaleId = dto.SaleId,
                TotalAmount = dto.TotalAmount,
                PaidAmount = 0,
                IssueDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Status = AccountReceivableStatus.Pending,
                Description = dto.Description,
                Notes = dto.Notes,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _accountsReceivableRepository.AddAsync(accountsReceivable);
            await _unitOfWork.SaveChangesAsync();

            return await GetAccountsReceivableByIdAsync(accountsReceivable.Id);
        }

        public async Task<AccountsReceivableResponseDto> GetAccountsReceivableByIdAsync(Guid id)
        {
            var accountsReceivable = await _accountsReceivableRepository
                .FirstOrDefaultAsync(ar => ar.Id == id);

            if (accountsReceivable == null)
                throw new ArgumentException("Cuenta por cobrar no encontrada");

            return MapToAccountsReceivableResponseDto(accountsReceivable);
        }

        public async Task<IEnumerable<AccountsReceivableResponseDto>> GetAccountsReceivableByCompanyAsync(Guid companyId)
        {
            var accountsReceivables = await _accountsReceivableRepository
                .WhereAsync(ar => ar.CompanyId == companyId);

            return accountsReceivables.Select(MapToAccountsReceivableResponseDto);
        }

        public async Task<IEnumerable<AccountsReceivableResponseDto>> GetOverdueAccountsReceivableAsync(Guid companyId)
        {
            var overdueReceivables = await _accountsReceivableRepository
                .WhereAsync(ar => ar.CompanyId == companyId && 
                                     ar.DueDate < DateTime.UtcNow && 
                                     ar.RemainingAmount > 0);

            return overdueReceivables.Select(MapToAccountsReceivableResponseDto);
        }

        public async Task<AccountsReceivableResponseDto> UpdateAccountsReceivableAsync(Guid id, CreateAccountsReceivableDto dto)
        {
            var accountsReceivable = await _accountsReceivableRepository.GetByIdAsync(id);
            if (accountsReceivable == null)
                throw new ArgumentException("Cuenta por cobrar no encontrada");

            accountsReceivable.InvoiceNumber = dto.InvoiceNumber;
            accountsReceivable.CustomerId = dto.CustomerId;
            accountsReceivable.SaleId = dto.SaleId;
            accountsReceivable.TotalAmount = dto.TotalAmount;
            accountsReceivable.DueDate = dto.DueDate;
            accountsReceivable.Description = dto.Description;
            accountsReceivable.Notes = dto.Notes;
            accountsReceivable.UpdatedAt = DateTime.UtcNow;

            _accountsReceivableRepository.Update(accountsReceivable);
            await _unitOfWork.SaveChangesAsync();

            return await GetAccountsReceivableByIdAsync(id);
        }

        public async Task<bool> DeleteAccountsReceivableAsync(Guid id)
        {
            var accountsReceivable = await _accountsReceivableRepository.GetByIdAsync(id);
            if (accountsReceivable == null)
                return false;

            if (accountsReceivable.PaidAmount > 0)
                throw new InvalidOperationException("No se puede eliminar una cuenta por cobrar con pagos registrados");

            _accountsReceivableRepository.Remove(accountsReceivable);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Accounts Receivable Payments

        public async Task<AccountsReceivablePaymentResponseDto> CreateAccountsReceivablePaymentAsync(CreateAccountsReceivablePaymentDto dto)
        {
            var accountsReceivable = await _accountsReceivableRepository.GetByIdAsync(dto.AccountsReceivableId);
            if (accountsReceivable == null)
                throw new ArgumentException("Cuenta por cobrar no encontrada");

            if (dto.Amount > accountsReceivable.RemainingAmount)
                throw new InvalidOperationException("El monto del pago no puede ser mayor al saldo pendiente");

            var payment = new AccountsReceivablePayment
            {
                Id = Guid.NewGuid(),
                AccountsReceivableId = dto.AccountsReceivableId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                ReferenceNumber = dto.ReferenceNumber,
                Notes = dto.Notes,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _accountsReceivablePaymentRepository.AddAsync(payment);

            // Actualizar la cuenta por cobrar
            accountsReceivable.PaidAmount += dto.Amount;
            accountsReceivable.Status = accountsReceivable.RemainingAmount == 0 ? 
                AccountReceivableStatus.Paid : AccountReceivableStatus.PartiallyPaid;
            accountsReceivable.UpdatedAt = DateTime.UtcNow;

            _accountsReceivableRepository.Update(accountsReceivable);
            await _unitOfWork.SaveChangesAsync();

            return MapToAccountsReceivablePaymentResponseDto(payment);
        }

        public async Task<IEnumerable<AccountsReceivablePaymentResponseDto>> GetPaymentsByAccountsReceivableAsync(Guid accountsReceivableId)
        {
            var payments = await _accountsReceivablePaymentRepository
                .WhereAsync(p => p.AccountsReceivableId == accountsReceivableId);

            var orderedPayments = payments.OrderByDescending(p => p.PaymentDate).ToList();

            return orderedPayments.Select(MapToAccountsReceivablePaymentResponseDto);
        }

        #endregion

        #region Private Helper Methods

        private AccountsReceivableResponseDto MapToAccountsReceivableResponseDto(AccountsReceivable accountsReceivable)
        {
            var isOverdue = accountsReceivable.DueDate < DateTime.UtcNow && accountsReceivable.RemainingAmount > 0;
            var daysOverdue = isOverdue ? (DateTime.UtcNow - accountsReceivable.DueDate).Days : 0;

            return new AccountsReceivableResponseDto
            {
                Id = accountsReceivable.Id,
                InvoiceNumber = accountsReceivable.InvoiceNumber,
                CustomerId = accountsReceivable.CustomerId,
                CustomerName = accountsReceivable.Customer?.Name ?? string.Empty,
                SaleId = accountsReceivable.SaleId,
                TotalAmount = accountsReceivable.TotalAmount,
                PaidAmount = accountsReceivable.PaidAmount,
                RemainingAmount = accountsReceivable.RemainingAmount,
                IssueDate = accountsReceivable.IssueDate,
                DueDate = accountsReceivable.DueDate,
                Status = accountsReceivable.Status,
                StatusName = accountsReceivable.Status.ToString(),
                Description = accountsReceivable.Description,
                Notes = accountsReceivable.Notes,
                IsOverdue = isOverdue,
                DaysOverdue = daysOverdue,
                CreatedAt = accountsReceivable.CreatedAt,
                UpdatedAt = accountsReceivable.UpdatedAt,
                Payments = accountsReceivable.Payments?.Select(MapToAccountsReceivablePaymentResponseDto).ToList() ?? new List<AccountsReceivablePaymentResponseDto>()
            };
        }

        private AccountsReceivablePaymentResponseDto MapToAccountsReceivablePaymentResponseDto(AccountsReceivablePayment payment)
        {
            return new AccountsReceivablePaymentResponseDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                PaymentMethodName = payment.PaymentMethod.ToString(),
                ReferenceNumber = payment.ReferenceNumber,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt
            };
        }

        private AccountsPayableResponseDto MapToAccountsPayableResponseDto(AccountsPayable accountsPayable)
        {
            var isOverdue = accountsPayable.DueDate < DateTime.UtcNow && accountsPayable.RemainingAmount > 0;
            var daysOverdue = isOverdue ? (DateTime.UtcNow - accountsPayable.DueDate).Days : 0;

            return new AccountsPayableResponseDto
            {
                Id = accountsPayable.Id,
                InvoiceNumber = accountsPayable.InvoiceNumber,
                SupplierId = accountsPayable.SupplierId,
                SupplierName = accountsPayable.Supplier?.Name ?? string.Empty,
                PurchaseId = accountsPayable.PurchaseId,
                TotalAmount = accountsPayable.TotalAmount,
                PaidAmount = accountsPayable.PaidAmount,
                RemainingAmount = accountsPayable.RemainingAmount,
                DueDate = accountsPayable.DueDate,
                Status = accountsPayable.Status,
                StatusName = accountsPayable.Status.ToString(),
                Description = accountsPayable.Description,
                Notes = accountsPayable.Notes,
                IsOverdue = isOverdue,
                DaysOverdue = daysOverdue,
                CreatedAt = accountsPayable.CreatedAt,
                UpdatedAt = accountsPayable.UpdatedAt,
                Payments = accountsPayable.Payments?.Select(MapToAccountsPayablePaymentResponseDto).ToList() ?? new List<AccountsPayablePaymentResponseDto>()
            };
        }

        private AccountsPayablePaymentResponseDto MapToAccountsPayablePaymentResponseDto(AccountsPayablePayment payment)
        {
            return new AccountsPayablePaymentResponseDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                PaymentMethodName = payment.PaymentMethod.ToString(),
                ReferenceNumber = payment.ReferenceNumber,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt
            };
        }

        #endregion

        #region Accounts Payable

        public async Task<AccountsPayableResponseDto> CreateAccountsPayableAsync(CreateAccountsPayableDto dto)
        {
            var accountsPayable = new AccountsPayable
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = dto.InvoiceNumber,
                SupplierId = dto.SupplierId,
                PurchaseId = dto.PurchaseId,
                TotalAmount = dto.TotalAmount,
                PaidAmount = 0,
                DueDate = dto.DueDate,
                Description = dto.Description,
                Notes = dto.Notes,
                Status = AccountPayableStatus.Pending,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _accountsPayableRepository.AddAsync(accountsPayable);
            await _unitOfWork.SaveChangesAsync();

            return MapToAccountsPayableResponseDto(accountsPayable);
        }

        public async Task<AccountsPayableResponseDto> GetAccountsPayableByIdAsync(Guid id)
        {
            var accountsPayable = await _accountsPayableRepository.GetByIdAsync(id);
            if (accountsPayable == null)
                throw new ArgumentException("Cuenta por pagar no encontrada");

            return MapToAccountsPayableResponseDto(accountsPayable);
        }

        public async Task<IEnumerable<AccountsPayableResponseDto>> GetAccountsPayableByCompanyAsync(Guid companyId)
        {
            var accountsPayable = await _accountsPayableRepository
                .WhereAsync(ar => ar.CompanyId == companyId);

            var orderedAccountsPayable = accountsPayable.OrderByDescending(ar => ar.CreatedAt).ToList();

            return orderedAccountsPayable.Select(MapToAccountsPayableResponseDto);
        }

        public async Task<IEnumerable<AccountsPayableResponseDto>> GetOverdueAccountsPayableAsync(Guid companyId)
        {
            var overdueAccountsPayable = await _accountsPayableRepository
                .WhereAsync(ar => ar.CompanyId == companyId && 
                                     ar.DueDate < DateTime.UtcNow && 
                                     ar.Status != AccountPayableStatus.Paid);

            var orderedOverdueAccountsPayable = overdueAccountsPayable.OrderBy(ar => ar.DueDate).ToList();

            return orderedOverdueAccountsPayable.Select(MapToAccountsPayableResponseDto);
        }

        public async Task<AccountsPayableResponseDto> UpdateAccountsPayableAsync(Guid id, CreateAccountsPayableDto dto)
        {
            var accountsPayable = await _accountsPayableRepository.GetByIdAsync(id);
            if (accountsPayable == null)
                throw new ArgumentException("Cuenta por pagar no encontrada");

            accountsPayable.InvoiceNumber = dto.InvoiceNumber;
            accountsPayable.SupplierId = dto.SupplierId;
            accountsPayable.PurchaseId = dto.PurchaseId;
            accountsPayable.TotalAmount = dto.TotalAmount;
            accountsPayable.DueDate = dto.DueDate;
            accountsPayable.Description = dto.Description;
            accountsPayable.Notes = dto.Notes;
            accountsPayable.UpdatedAt = DateTime.UtcNow;

            _accountsPayableRepository.Update(accountsPayable);
            await _unitOfWork.SaveChangesAsync();

            return MapToAccountsPayableResponseDto(accountsPayable);
        }

        public async Task<bool> DeleteAccountsPayableAsync(Guid id)
        {
            var accountsPayable = await _accountsPayableRepository.GetByIdAsync(id);
            if (accountsPayable == null)
                return false;

            if (accountsPayable.PaidAmount > 0)
                throw new InvalidOperationException("No se puede eliminar una cuenta por pagar que tiene pagos registrados");

            _accountsPayableRepository.Remove(accountsPayable);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Accounts Payable Payments

        public async Task<AccountsPayablePaymentResponseDto> CreateAccountsPayablePaymentAsync(CreateAccountsPayablePaymentDto dto)
        {
            var accountsPayable = await _accountsPayableRepository.GetByIdAsync(dto.AccountsPayableId);
            if (accountsPayable == null)
                throw new ArgumentException("Cuenta por pagar no encontrada");

            if (dto.Amount > accountsPayable.RemainingAmount)
                throw new InvalidOperationException("El monto del pago no puede ser mayor al saldo pendiente");

            var payment = new AccountsPayablePayment
            {
                Id = Guid.NewGuid(),
                AccountsPayableId = dto.AccountsPayableId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                ReferenceNumber = dto.ReferenceNumber,
                Notes = dto.Notes,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _accountsPayablePaymentRepository.AddAsync(payment);

            // Actualizar la cuenta por pagar
            accountsPayable.PaidAmount += dto.Amount;
            accountsPayable.Status = accountsPayable.RemainingAmount == 0 ? 
                AccountPayableStatus.Paid : AccountPayableStatus.PartiallyPaid;
            accountsPayable.UpdatedAt = DateTime.UtcNow;

            _accountsPayableRepository.Update(accountsPayable);
            await _unitOfWork.SaveChangesAsync();

            return MapToAccountsPayablePaymentResponseDto(payment);
        }

        public async Task<IEnumerable<AccountsPayablePaymentResponseDto>> GetPaymentsByAccountsPayableAsync(Guid accountsPayableId)
        {
            var payments = await _accountsPayablePaymentRepository
                .WhereAsync(p => p.AccountsPayableId == accountsPayableId);

            var orderedPayments = payments.OrderByDescending(p => p.PaymentDate).ToList();

            return orderedPayments.Select(MapToAccountsPayablePaymentResponseDto);
        }

        #endregion

        #region Budget

        public async Task<BudgetResponseDto> CreateBudgetAsync(CreateBudgetDto dto)
        {
            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Year = dto.Year,
                Month = dto.Month,
                BudgetedAmount = dto.BudgetedAmount,
                ActualAmount = 0,
                Category = dto.Category,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _budgetRepository.AddAsync(budget);

            // Crear líneas de presupuesto si existen
            if (dto.BudgetLines?.Any() == true)
            {
                var budgetLines = dto.BudgetLines.Select(bl => new BudgetLine
                {
                    Id = Guid.NewGuid(),
                    BudgetId = budget.Id,
                    Description = bl.Description,
                    BudgetedAmount = bl.BudgetedAmount,
                    ActualAmount = 0,
                    Notes = bl.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                foreach (var budgetLine in budgetLines)
                {
                    await _budgetLineRepository.AddAsync(budgetLine);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToBudgetResponseDto(budget);
        }

        public async Task<BudgetResponseDto> GetBudgetByIdAsync(Guid id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null)
                throw new ArgumentException("Presupuesto no encontrado");

            return MapToBudgetResponseDto(budget);
        }

        public async Task<IEnumerable<BudgetResponseDto>> GetBudgetsByCompanyAsync(Guid companyId, int? year = null, int? month = null)
        {
            var budgets = await _budgetRepository.WhereAsync(b => b.CompanyId == companyId);
            var query = budgets.AsQueryable();
            
            if (year.HasValue)
                query = query.Where(b => b.Year == year.Value);
                
            if (month.HasValue)
                query = query.Where(b => b.Month == month.Value);

            var orderedBudgets = query
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ToList();

            return orderedBudgets.Select(MapToBudgetResponseDto);
        }

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(Guid companyId, int year, int? month = null)
        {
            var budgets = await _budgetRepository.WhereAsync(b => b.CompanyId == companyId && b.Year == year);
            var query = budgets.AsQueryable();
            
            if (month.HasValue)
                query = query.Where(b => b.Month == month.Value);

            var filteredBudgets = query.ToList();

            var totalBudgeted = filteredBudgets.Sum(b => b.BudgetedAmount);
            var totalActual = filteredBudgets.Sum(b => b.ActualAmount);
            var variance = totalActual - totalBudgeted;
            var variancePercentage = totalBudgeted > 0 ? (variance / totalBudgeted) * 100 : 0;

            return new BudgetSummaryDto
            {
                TotalBudgeted = totalBudgeted,
                TotalActual = totalActual,
                TotalVariance = variance,
                VariancePercentage = variancePercentage,
                BudgetCount = filteredBudgets.Count,
                OverBudgetCount = filteredBudgets.Count(b => b.ActualAmount > b.BudgetedAmount),
                UnderBudgetCount = filteredBudgets.Count(b => b.ActualAmount < b.BudgetedAmount),
                PeriodStart = new DateTime(year, month ?? 1, 1),
                PeriodEnd = month.HasValue ? new DateTime(year, month.Value, DateTime.DaysInMonth(year, month.Value)) : new DateTime(year, 12, 31)
            };
        }

        public async Task<BudgetResponseDto> UpdateBudgetAsync(Guid id, CreateBudgetDto dto)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null)
                throw new ArgumentException("Presupuesto no encontrado");

            budget.Name = dto.Name;
            budget.Description = dto.Description;
            budget.Year = dto.Year;
            budget.Month = dto.Month;
            budget.BudgetedAmount = dto.BudgetedAmount;
            budget.Category = dto.Category;
            budget.UpdatedAt = DateTime.UtcNow;

            _budgetRepository.Update(budget);
            await _unitOfWork.SaveChangesAsync();

            return MapToBudgetResponseDto(budget);
        }

        public async Task<bool> DeleteBudgetAsync(Guid id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null)
                return false;

            // Eliminar líneas de presupuesto asociadas
            var budgetLines = await _budgetLineRepository
                .WhereAsync(bl => bl.BudgetId == id);

            if (budgetLines.Any())
            {
                foreach (var budgetLine in budgetLines)
                {
                    _budgetLineRepository.Remove(budgetLine);
                }
            }

            _budgetRepository.Remove(budget);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalAccountsReceivableAsync(Guid companyId)
        {
            var receivables = await _accountsReceivableRepository
                .WhereAsync(ar => ar.CompanyId == companyId);
            return receivables.Sum(ar => ar.RemainingAmount);
        }

        public async Task<decimal> GetTotalAccountsPayableAsync(Guid companyId)
        {
            var payables = await _accountsPayableRepository
                .WhereAsync(ap => ap.CompanyId == companyId);
            return payables.Sum(ap => ap.RemainingAmount);
        }

        public async Task<decimal> GetNetReceivablesAsync(Guid companyId)
        {
            var totalReceivable = await GetTotalAccountsReceivableAsync(companyId);
            var totalPayable = await GetTotalAccountsPayableAsync(companyId);
            return totalReceivable - totalPayable;
        }

        public async Task<object> GetAgingReportAsync(Guid companyId, bool isReceivable = true)
        {
            if (isReceivable)
            {
                var receivables = await _accountsReceivableRepository
                    .WhereAsync(ar => ar.CompanyId == companyId && ar.RemainingAmount > 0);

                return new
                {
                    Current = receivables.Where(r => r.DueDate >= DateTime.UtcNow).Sum(r => r.RemainingAmount),
                    Days1To30 = receivables.Where(r => r.DueDate < DateTime.UtcNow && (DateTime.UtcNow - r.DueDate).Days <= 30).Sum(r => r.RemainingAmount),
                    Days31To60 = receivables.Where(r => (DateTime.UtcNow - r.DueDate).Days > 30 && (DateTime.UtcNow - r.DueDate).Days <= 60).Sum(r => r.RemainingAmount),
                    Days61To90 = receivables.Where(r => (DateTime.UtcNow - r.DueDate).Days > 60 && (DateTime.UtcNow - r.DueDate).Days <= 90).Sum(r => r.RemainingAmount),
                    Over90Days = receivables.Where(r => (DateTime.UtcNow - r.DueDate).Days > 90).Sum(r => r.RemainingAmount)
                };
            }
            else
            {
                var payables = await _accountsPayableRepository
                    .WhereAsync(ap => ap.CompanyId == companyId && ap.RemainingAmount > 0);

                return new
                {
                    Current = payables.Where(p => p.DueDate >= DateTime.UtcNow).Sum(p => p.RemainingAmount),
                    Days1To30 = payables.Where(p => p.DueDate < DateTime.UtcNow && (DateTime.UtcNow - p.DueDate).Days <= 30).Sum(p => p.RemainingAmount),
                    Days31To60 = payables.Where(p => (DateTime.UtcNow - p.DueDate).Days > 30 && (DateTime.UtcNow - p.DueDate).Days <= 60).Sum(p => p.RemainingAmount),
                    Days61To90 = payables.Where(p => (DateTime.UtcNow - p.DueDate).Days > 60 && (DateTime.UtcNow - p.DueDate).Days <= 90).Sum(p => p.RemainingAmount),
                    Over90Days = payables.Where(p => (DateTime.UtcNow - p.DueDate).Days > 90).Sum(p => p.RemainingAmount)
                };
            }
        }

        private BudgetResponseDto MapToBudgetResponseDto(Budget budget)
         {
             return new BudgetResponseDto
             {
                 Id = budget.Id,
                 Name = budget.Name,
                 Description = budget.Description,
                 Year = budget.Year,
                 Month = budget.Month,
                 BudgetedAmount = budget.BudgetedAmount,
                 ActualAmount = budget.ActualAmount,
                 Variance = budget.ActualAmount - budget.BudgetedAmount,
                 VariancePercentage = budget.BudgetedAmount > 0 ? ((budget.ActualAmount - budget.BudgetedAmount) / budget.BudgetedAmount) * 100 : 0,
                 Category = budget.Category,
                 CreatedAt = budget.CreatedAt,
                 UpdatedAt = budget.UpdatedAt
             };
         }

         #endregion
     }
 }