using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class BankReconciliationService : IBankReconciliationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<BankAccount> _bankAccountRepository;
        private readonly IRepository<BankTransaction> _bankTransactionRepository;
        private readonly IRepository<BankReconciliation> _bankReconciliationRepository;
        private readonly IRepository<BankReconciliationItem> _bankReconciliationItemRepository;

        public BankReconciliationService(
            IUnitOfWork unitOfWork,
            IRepository<BankAccount> bankAccountRepository,
            IRepository<BankTransaction> bankTransactionRepository,
            IRepository<BankReconciliation> bankReconciliationRepository,
            IRepository<BankReconciliationItem> bankReconciliationItemRepository)
        {
            _unitOfWork = unitOfWork;
            _bankAccountRepository = bankAccountRepository;
            _bankTransactionRepository = bankTransactionRepository;
            _bankReconciliationRepository = bankReconciliationRepository;
            _bankReconciliationItemRepository = bankReconciliationItemRepository;
        }

        #region Bank Account Management

        public async Task<BankAccountResponseDto> CreateBankAccountAsync(CreateBankAccountDto dto)
        {
            var bankAccount = new BankAccount
            {
                Id = Guid.NewGuid(),
                AccountNumber = dto.AccountNumber,
                BankName = dto.BankName,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                Currency = dto.Currency,
                Balance = dto.InitialBalance,
                IsActive = true,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _bankAccountRepository.AddAsync(bankAccount);
            await _unitOfWork.SaveChangesAsync();

            return MapToBankAccountResponseDto(bankAccount);
        }

        public async Task<BankAccountResponseDto> GetBankAccountByIdAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(id);
            if (bankAccount == null)
                throw new ArgumentException("Cuenta bancaria no encontrada");

            return MapToBankAccountResponseDto(bankAccount);
        }

        public async Task<IEnumerable<BankAccountResponseDto>> GetBankAccountsByCompanyAsync(Guid companyId)
        {
            var bankAccountsQuery = await _bankAccountRepository
                .WhereAsync(ba => ba.CompanyId == companyId);
            var bankAccounts = bankAccountsQuery
                .OrderBy(ba => ba.BankName)
                .ThenBy(ba => ba.AccountName)
                .ToList();

            return bankAccounts.Select(MapToBankAccountResponseDto);
        }

        public async Task<BankAccountResponseDto> UpdateBankAccountAsync(Guid id, CreateBankAccountDto dto)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(id);
            if (bankAccount == null)
                throw new ArgumentException("Cuenta bancaria no encontrada");

            bankAccount.AccountNumber = dto.AccountNumber;
            bankAccount.BankName = dto.BankName;
            bankAccount.AccountName = dto.AccountName;
            bankAccount.AccountType = dto.AccountType;
            bankAccount.Currency = dto.Currency;
            bankAccount.UpdatedAt = DateTime.UtcNow;

            _bankAccountRepository.Update(bankAccount);
            await _unitOfWork.SaveChangesAsync();

            return MapToBankAccountResponseDto(bankAccount);
        }

        public async Task<bool> DeleteBankAccountAsync(Guid id)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(id);
            if (bankAccount == null)
                return false;

            // Verificar si tiene transacciones
        var transactions = await _bankTransactionRepository
            .WhereAsync(bt => bt.BankAccountId == id);
        var hasTransactions = transactions.Any();

            if (hasTransactions)
                throw new InvalidOperationException("No se puede eliminar una cuenta bancaria con transacciones registradas");

            _bankAccountRepository.Remove(bankAccount);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Bank Transaction Management

        public async Task<BankTransactionResponseDto> CreateBankTransactionAsync(CreateBankTransactionDto dto)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(dto.BankAccountId);
            if (bankAccount == null)
                throw new ArgumentException("Cuenta bancaria no encontrada");

            var transaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = dto.BankAccountId,
                TransactionDate = dto.TransactionDate,
                Description = dto.Description,
                Amount = dto.Amount,
                Type = dto.Type,
                ReferenceNumber = dto.ReferenceNumber,
                IsReconciled = false,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _bankTransactionRepository.AddAsync(transaction);

            // Actualizar el saldo de la cuenta bancaria
            if (dto.Type == BankTransactionType.Deposit)
                bankAccount.Balance += dto.Amount;
            else
                bankAccount.Balance -= dto.Amount;

            bankAccount.UpdatedAt = DateTime.UtcNow;
            _bankAccountRepository.Update(bankAccount);

            await _unitOfWork.SaveChangesAsync();

            return MapToBankTransactionResponseDto(transaction, bankAccount);
        }

        public async Task<BankTransactionResponseDto> GetBankTransactionByIdAsync(Guid id)
        {
            var transaction = await _bankTransactionRepository
                .FirstOrDefaultAsync(bt => bt.Id == id);

            if (transaction == null)
                throw new ArgumentException("Transacción bancaria no encontrada");

            return MapToBankTransactionResponseDto(transaction, transaction.BankAccount);
        }

        public async Task<IEnumerable<BankTransactionResponseDto>> GetBankTransactionsByAccountAsync(Guid bankAccountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = await _bankTransactionRepository
                .WhereAsync(bt => bt.BankAccountId == bankAccountId);

            if (startDate.HasValue)
                query = query.Where(bt => bt.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(bt => bt.TransactionDate <= endDate.Value);

            var transactions = query
                .OrderByDescending(bt => bt.TransactionDate)
                .ToList();

            return transactions.Select(t => MapToBankTransactionResponseDto(t, t.BankAccount));
        }

        public async Task<IEnumerable<BankTransactionResponseDto>> GetUnreconciledTransactionsAsync(Guid bankAccountId)
        {
            var unreconciledTransactions = await _bankTransactionRepository
                .WhereAsync(bt => bt.BankAccountId == bankAccountId && !bt.IsReconciled);

            return unreconciledTransactions.Select(t => MapToBankTransactionResponseDto(t, t.BankAccount));
        }

        public async Task<BankTransactionResponseDto> UpdateBankTransactionAsync(Guid id, CreateBankTransactionDto dto)
        {
            var transaction = await _bankTransactionRepository
                .FirstOrDefaultAsync(bt => bt.Id == id);

            if (transaction == null)
                throw new ArgumentException("Transacción bancaria no encontrada");

            if (transaction.IsReconciled)
                throw new InvalidOperationException("No se puede modificar una transacción ya conciliada");

            var bankAccount = transaction.BankAccount;
            var oldAmount = transaction.Amount;
            var oldType = transaction.Type;

            // Revertir el efecto de la transacción anterior en el saldo
            if (oldType == BankTransactionType.Deposit)
                bankAccount.Balance -= oldAmount;
            else
                bankAccount.Balance += oldAmount;

            // Actualizar la transacción
            transaction.TransactionDate = dto.TransactionDate;
            transaction.Description = dto.Description;
            transaction.Amount = dto.Amount;
            transaction.Type = dto.Type;
            transaction.ReferenceNumber = dto.ReferenceNumber;
            // UpdatedAt property removed from BankTransaction

            // Aplicar el nuevo efecto en el saldo
            if (dto.Type == BankTransactionType.Deposit)
                bankAccount.Balance += dto.Amount;
            else
                bankAccount.Balance -= dto.Amount;

            bankAccount.UpdatedAt = DateTime.UtcNow;

            _bankTransactionRepository.Update(transaction);
            _bankAccountRepository.Update(bankAccount);
            await _unitOfWork.SaveChangesAsync();

            return MapToBankTransactionResponseDto(transaction, bankAccount);
        }

        public async Task<bool> DeleteBankTransactionAsync(Guid id)
        {
            var transaction = await _bankTransactionRepository
                .FirstOrDefaultAsync(bt => bt.Id == id);

            if (transaction == null)
                return false;

            if (transaction.IsReconciled)
                throw new InvalidOperationException("No se puede eliminar una transacción ya conciliada");

            var bankAccount = transaction.BankAccount;

            // Revertir el efecto en el saldo
            if (transaction.Type == BankTransactionType.Deposit)
                bankAccount.Balance -= transaction.Amount;
            else
                bankAccount.Balance += transaction.Amount;

            bankAccount.UpdatedAt = DateTime.UtcNow;

            _bankTransactionRepository.Remove(transaction);
            _bankAccountRepository.Update(bankAccount);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Bank Reconciliation

        public async Task<object> StartReconciliationAsync(Guid bankAccountId, DateTime reconciliationDate, decimal statementBalance)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException("Cuenta bancaria no encontrada");

            var reconciliation = new BankReconciliation
            {
                Id = Guid.NewGuid(),
                BankAccountId = bankAccountId,
                ReconciliationDate = reconciliationDate,
                StatementBalance = statementBalance,
                BookBalance = bankAccount.Balance,
                Status = ReconciliationStatus.InProgress,
                CompanyId = bankAccount.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _bankReconciliationRepository.AddAsync(reconciliation);
            await _unitOfWork.SaveChangesAsync();

            return new { ReconciliationId = reconciliation.Id, Message = "Conciliación iniciada exitosamente" };
        }

        public async Task<object> CompleteReconciliationAsync(Guid reconciliationId)
        {
            var reconciliation = await _bankReconciliationRepository
                .FirstOrDefaultAsync(br => br.Id == reconciliationId);

            if (reconciliation == null)
                throw new ArgumentException("Conciliación no encontrada");

            if (reconciliation.Status == ReconciliationStatus.Completed)
                throw new InvalidOperationException("La conciliación ya está completada");

            // Marcar transacciones como conciliadas
            var transactionsQuery = await _bankTransactionRepository
                .WhereAsync(bt => bt.BankAccountId == reconciliation.BankAccountId && !bt.IsReconciled);
            var transactions = transactionsQuery.ToList();

            foreach (var transaction in transactions)
            {
                transaction.IsReconciled = true;
                _bankTransactionRepository.Update(transaction);
            }

            reconciliation.Status = ReconciliationStatus.Completed;
            reconciliation.UpdatedAt = DateTime.UtcNow;
            _bankReconciliationRepository.Update(reconciliation);
            await _unitOfWork.SaveChangesAsync();

            return new { Message = "Conciliación completada exitosamente" };
        }

        public async Task<IEnumerable<object>> GetReconciliationHistoryAsync(Guid bankAccountId)
        {
            var reconciliations = await _bankReconciliationRepository
                .WhereAsync(br => br.BankAccountId == bankAccountId);

            return reconciliations.Select(r => new
            {
                r.Id,
                r.ReconciliationDate,
                r.StatementBalance,
                r.BookBalance,
                Difference = r.StatementBalance - r.BookBalance,
                r.Status,
                r.UpdatedAt,
                r.CreatedAt
            });
        }

        #endregion

        #region Balance Management

        public async Task<decimal> GetCurrentBalanceAsync(Guid bankAccountId)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException("Cuenta bancaria no encontrada");

            return bankAccount.Balance;
        }

        public async Task<object> GetBalanceHistoryAsync(Guid bankAccountId, DateTime startDate, DateTime endDate)
        {
            var transactionsQuery = await _bankTransactionRepository
                .WhereAsync(bt => bt.BankAccountId == bankAccountId && 
                                     bt.TransactionDate >= startDate && 
                                     bt.TransactionDate <= endDate);
            var transactions = transactionsQuery.OrderBy(bt => bt.TransactionDate).ToList();

            var bankAccount = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            var runningBalance = bankAccount.Balance;
            var balanceHistory = new List<object>();

            foreach (var transaction in transactions)
            {
                if (transaction.Type == BankTransactionType.Deposit)
                    runningBalance += transaction.Amount;
                else
                    runningBalance -= transaction.Amount;

                balanceHistory.Add(new
                {
                    Date = transaction.TransactionDate,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    Type = transaction.Type.ToString(),
                    Balance = runningBalance
                });
            }

            return balanceHistory;
        }

        #endregion

        #region Bank Statement Import

        public async Task<object> ImportBankStatementAsync(Guid bankAccountId, IEnumerable<object> statementData)
        {
            // TODO: Implementar importación de estados de cuenta
            throw new NotImplementedException("Funcionalidad de importación de estados de cuenta pendiente de implementación");
        }

        #endregion

        #region Private Helper Methods

        private BankAccountResponseDto MapToBankAccountResponseDto(BankAccount bankAccount)
        {
            return new BankAccountResponseDto
            {
                Id = bankAccount.Id,
                AccountNumber = bankAccount.AccountNumber,
                BankName = bankAccount.BankName,
                AccountName = bankAccount.AccountName,
                AccountType = bankAccount.AccountType.ToString(),
                Currency = bankAccount.Currency,
                Balance = bankAccount.Balance,
                IsActive = bankAccount.IsActive,
                CreatedAt = bankAccount.CreatedAt,
                UpdatedAt = bankAccount.UpdatedAt
            };
        }

        private BankTransactionResponseDto MapToBankTransactionResponseDto(BankTransaction transaction, BankAccount bankAccount)
        {
            return new BankTransactionResponseDto
            {
                Id = transaction.Id,
                BankAccountId = transaction.BankAccountId,
                BankAccountName = bankAccount?.AccountName ?? string.Empty,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Type = transaction.Type,
                TypeName = transaction.Type.ToString(),
                ReferenceNumber = transaction.ReferenceNumber,
                IsReconciled = transaction.IsReconciled,
                CreatedAt = transaction.CreatedAt
            };
        }

        #endregion

        #region Bank Statement Import

        public async Task<IEnumerable<BankTransactionResponseDto>> ImportBankStatementAsync(Guid bankAccountId, Stream fileStream, string fileName)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException("Bank account not found");

            var transactions = new List<BankTransaction>();
            
            // Simple CSV parsing implementation
            using var reader = new StreamReader(fileStream);
            string? line;
            bool isFirstLine = true;
            
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Skip header
                }
                
                var columns = line.Split(',');
                if (columns.Length >= 4)
                {
                    if (DateTime.TryParse(columns[0], out var date) &&
                        decimal.TryParse(columns[2], out var amount))
                    {
                        var transaction = new BankTransaction
                        {
                            Id = Guid.NewGuid(),
                            BankAccountId = bankAccountId,
                            CompanyId = bankAccount.CompanyId,
                            TransactionDate = date,
                            Description = columns[1],
                            Amount = Math.Abs(amount),
                            Type = amount >= 0 ? BankTransactionType.Deposit : BankTransactionType.Withdrawal,
                            ReferenceNumber = columns.Length > 3 ? columns[3] : null,
                            IsReconciled = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        transactions.Add(transaction);
                    }
                }
            }
            
            // Save transactions
            foreach (var transaction in transactions)
            {
                await _bankTransactionRepository.AddAsync(transaction);
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            return transactions.Select(t => MapToBankTransactionResponseDto(t, bankAccount));
        }

        public async Task<object> ValidateBankStatementAsync(Guid bankAccountId, Stream fileStream, string fileName)
        {
            var bankAccount = await _bankAccountRepository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException("Bank account not found");

            var validationResults = new List<object>();
            var lineNumber = 0;
            
            using var reader = new StreamReader(fileStream);
            string? line;
            bool isFirstLine = true;
            
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // Skip header
                }
                
                var columns = line.Split(',');
                var errors = new List<string>();
                
                if (columns.Length < 4)
                {
                    errors.Add("Insufficient columns");
                }
                else
                {
                    if (!DateTime.TryParse(columns[0], out _))
                        errors.Add("Invalid date format");
                    
                    if (string.IsNullOrWhiteSpace(columns[1]))
                        errors.Add("Description is required");
                    
                    if (!decimal.TryParse(columns[2], out _))
                        errors.Add("Invalid amount format");
                }
                
                validationResults.Add(new
                {
                    LineNumber = lineNumber,
                    IsValid = errors.Count == 0,
                    Errors = errors,
                    RawData = line
                });
            }
            
            return new
            {
                FileName = fileName,
                TotalLines = lineNumber,
                ValidLines = validationResults.Count(r => ((dynamic)r).IsValid),
                InvalidLines = validationResults.Count(r => !((dynamic)r).IsValid),
                ValidationResults = validationResults
            };
        }

        #endregion

        #region Bank Account Management

        public async Task<bool> ActivateDeactivateBankAccountAsync(Guid accountId, bool isActive)
        {
            var account = await _bankAccountRepository.GetByIdAsync(accountId);
            if (account == null)
                return false;

            account.IsActive = isActive;
            account.UpdatedAt = DateTime.UtcNow;
            
            _bankAccountRepository.Update(account);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Transaction Reconciliation

        public async Task<bool> ReconcileTransactionAsync(Guid transactionId, DateTime reconciliationDate)
        {
            var transaction = await _bankTransactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                return false;

            transaction.IsReconciled = true;
            transaction.ReconciledDate = reconciliationDate;
            
            _bankTransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReconcileMultipleTransactionsAsync(List<Guid> transactionIds, DateTime reconciliationDate)
        {
            var transactionsQuery = await _bankTransactionRepository
                .WhereAsync(bt => transactionIds.Contains(bt.Id));
            var transactions = transactionsQuery.ToList();

            foreach (var transaction in transactions)
            {
                transaction.IsReconciled = true;
                transaction.ReconciledDate = reconciliationDate;
            }

            foreach (var transaction in transactions)
            {
                _bankTransactionRepository.Update(transaction);
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnreconcileTransactionAsync(Guid transactionId)
        {
            var transaction = await _bankTransactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                return false;

            transaction.IsReconciled = false;
            transaction.ReconciledDate = null;
            
            _bankTransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Reports and Balance Methods

        public async Task<object> GetReconciliationReportAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var reconciliations = await _bankReconciliationRepository
                .WhereAsync(br => br.CompanyId == companyId &&
                                     br.ReconciliationDate >= startDate &&
                                     br.ReconciliationDate <= endDate);

            var totalReconciled = reconciliations.Sum(r => r.ReconciledBalance);
            var totalItems = reconciliations.SelectMany(r => r.ReconciliationItems).Count();

            return new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                TotalReconciliations = reconciliations.Count(),
                TotalReconciledAmount = totalReconciled,
                TotalItems = totalItems,
                Reconciliations = reconciliations.Select(r => new
                {
                    r.Id,
                    r.ReconciliationDate,
                    r.StatementBalance,
                    r.ReconciledBalance,
                    r.Status,
                    ItemsCount = r.ReconciliationItems.Count()
                })
            };
        }

        public async Task<decimal> GetReconciledBalanceAsync(Guid companyId)
        {
            var reconciliations = await _bankReconciliationRepository
                .WhereAsync(br => br.CompanyId == companyId);
            
            var latestReconciliation = reconciliations
                .OrderByDescending(br => br.ReconciliationDate)
                .FirstOrDefault();

            return latestReconciliation?.ReconciledBalance ?? 0;
        }

        public async Task<decimal> GetUnreconciledBalanceAsync(Guid companyId)
        {
            var currentBalance = await GetCurrentBalanceAsync(companyId);
            var reconciledBalance = await GetReconciledBalanceAsync(companyId);
            
            return currentBalance - reconciledBalance;
        }

        #endregion
    }
}