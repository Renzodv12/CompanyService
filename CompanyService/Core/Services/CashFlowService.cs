using CompanyService.Core.DTOs.Finance;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Core.Services
{
    public class CashFlowService : ICashFlowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CashFlow> _cashFlowRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<BankAccount> _bankAccountRepository;
        private readonly IRepository<BankTransaction> _bankTransactionRepository;
        private readonly IRepository<AccountsReceivable> _accountsReceivableRepository;
        private readonly IRepository<AccountsPayable> _accountsPayableRepository;

        public CashFlowService(
            IUnitOfWork unitOfWork,
            IRepository<CashFlow> cashFlowRepository,
            IRepository<Account> accountRepository,
            IRepository<BankAccount> bankAccountRepository,
            IRepository<BankTransaction> bankTransactionRepository,
            IRepository<AccountsReceivable> accountsReceivableRepository,
            IRepository<AccountsPayable> accountsPayableRepository)
        {
            _unitOfWork = unitOfWork;
            _cashFlowRepository = cashFlowRepository;
            _accountRepository = accountRepository;
            _bankAccountRepository = bankAccountRepository;
            _bankTransactionRepository = bankTransactionRepository;
            _accountsReceivableRepository = accountsReceivableRepository;
            _accountsPayableRepository = accountsPayableRepository;
        }

        #region Cash Flow Management

        public async Task<CashFlowResponseDto> CreateCashFlowAsync(CreateCashFlowDto dto)
        {
            var cashFlow = new CashFlow
            {
                Id = Guid.NewGuid(),
                Description = dto.Description,
                Type = dto.Type,
                Amount = dto.Amount,
                IsInflow = dto.IsInflow,
                TransactionDate = dto.TransactionDate,
                Category = dto.Category,
                ReferenceNumber = dto.ReferenceNumber,
                RelatedAccountId = dto.RelatedAccountId,
                RelatedBankAccountId = dto.RelatedBankAccountId,
                Notes = dto.Notes,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _cashFlowRepository.AddAsync(cashFlow);
            await _unitOfWork.SaveChangesAsync();

            return await GetCashFlowByIdAsync(cashFlow.Id);
        }

        public async Task<CashFlowResponseDto> GetCashFlowByIdAsync(Guid id)
        {
            var cashFlow = await _cashFlowRepository
                .FirstOrDefaultAsync(cf => cf.Id == id);

            if (cashFlow == null)
                throw new ArgumentException("Flujo de caja no encontrado");

            return MapToCashFlowResponseDto(cashFlow);
        }

        public async Task<IEnumerable<CashFlowResponseDto>> GetCashFlowsByCompanyAsync(Guid companyId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var cashFlowsQuery = await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId &&
                    (!startDate.HasValue || cf.TransactionDate >= startDate.Value) &&
                    (!endDate.HasValue || cf.TransactionDate <= endDate.Value));
            
            var cashFlows = cashFlowsQuery
                .OrderByDescending(cf => cf.TransactionDate)
                .ToList();

            return cashFlows.Select(MapToCashFlowResponseDto);
        }

        public async Task<IEnumerable<CashFlowResponseDto>> GetCashFlowsByTypeAsync(Guid companyId, CashFlowType type, DateTime? startDate = null, DateTime? endDate = null)
        {
            var cashFlowsQuery = await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && cf.Type == type &&
                    (!startDate.HasValue || cf.TransactionDate >= startDate.Value) &&
                    (!endDate.HasValue || cf.TransactionDate <= endDate.Value));

            var cashFlows = cashFlowsQuery
                .OrderByDescending(cf => cf.TransactionDate)
                .ToList();

            return cashFlows.Select(MapToCashFlowResponseDto);
        }

        public async Task<CashFlowResponseDto> UpdateCashFlowAsync(Guid id, CreateCashFlowDto dto)
        {
            var cashFlow = await _cashFlowRepository.GetByIdAsync(id);
            if (cashFlow == null)
                throw new ArgumentException("Flujo de caja no encontrado");

            cashFlow.Description = dto.Description;
            cashFlow.Type = dto.Type;
            cashFlow.Amount = dto.Amount;
            cashFlow.IsInflow = dto.IsInflow;
            cashFlow.TransactionDate = dto.TransactionDate;
            cashFlow.Category = dto.Category;
            cashFlow.ReferenceNumber = dto.ReferenceNumber;
            cashFlow.RelatedAccountId = dto.RelatedAccountId;
            cashFlow.RelatedBankAccountId = dto.RelatedBankAccountId;
            cashFlow.Notes = dto.Notes;
            cashFlow.UpdatedAt = DateTime.UtcNow;

            _cashFlowRepository.Update(cashFlow);
            await _unitOfWork.SaveChangesAsync();

            return await GetCashFlowByIdAsync(id);
        }

        public async Task<bool> DeleteCashFlowAsync(Guid id)
        {
            var cashFlow = await _cashFlowRepository.GetByIdAsync(id);
            if (cashFlow == null)
                return false;

            _cashFlowRepository.Remove(cashFlow);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Cash Flow Analysis

        public async Task<CashFlowSummaryDto> GetCashFlowSummaryAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var cashFlows = await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                      cf.TransactionDate >= startDate && 
                                      cf.TransactionDate <= endDate);

            var totalInflow = cashFlows.Where(cf => cf.IsInflow).Sum(cf => cf.Amount);
            var totalOutflow = cashFlows.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount);
            var netCashFlow = totalInflow - totalOutflow;

            var operatingCashFlow = cashFlows
                .Where(cf => cf.Type == CashFlowType.Operating)
                .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount);

            var investingCashFlow = cashFlows
                .Where(cf => cf.Type == CashFlowType.Investing)
                .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount);

            var financingCashFlow = cashFlows
                .Where(cf => cf.Type == CashFlowType.Financing)
                .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount);

            return new CashFlowSummaryDto
            {
                TotalInflow = totalInflow,
                TotalOutflow = totalOutflow,
                NetCashFlow = netCashFlow,
                OperatingCashFlow = operatingCashFlow,
                InvestingCashFlow = investingCashFlow,
                FinancingCashFlow = financingCashFlow,
                PeriodStart = startDate,
                PeriodEnd = endDate
            };
        }

        public async Task<CashFlowSummaryDto> GetMonthlyCashFlowSummaryAsync(Guid companyId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await GetCashFlowSummaryAsync(companyId, startDate, endDate);
        }

        public async Task<CashFlowSummaryDto> GetYearlyCashFlowSummaryAsync(Guid companyId, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            return await GetCashFlowSummaryAsync(companyId, startDate, endDate);
        }

        public async Task<IEnumerable<CashFlowSummaryDto>> GetCashFlowTrendAsync(Guid companyId, DateTime startDate, DateTime endDate, string period = "monthly")
        {
            var cashFlowsQuery = await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate);
            var cashFlows = cashFlowsQuery.ToList();

            if (period.ToLower() == "monthly")
            {
                var monthlyTrends = cashFlows
                    .GroupBy(cf => new { cf.TransactionDate.Year, cf.TransactionDate.Month })
                    .Select(g => new CashFlowSummaryDto
                    {
                        TotalInflow = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                        TotalOutflow = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                        NetCashFlow = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        OperatingCashFlow = g.Where(cf => cf.Type == CashFlowType.Operating)
                                            .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        InvestingCashFlow = g.Where(cf => cf.Type == CashFlowType.Investing)
                                            .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        FinancingCashFlow = g.Where(cf => cf.Type == CashFlowType.Financing)
                                            .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        PeriodStart = new DateTime(g.Key.Year, g.Key.Month, 1),
                        PeriodEnd = new DateTime(g.Key.Year, g.Key.Month, DateTime.DaysInMonth(g.Key.Year, g.Key.Month))
                    })
                    .OrderBy(t => t.PeriodStart)
                    .ToList();

                return monthlyTrends;
            }
            else
            {
                // Yearly trends
                var yearlyTrends = cashFlows
                    .GroupBy(cf => cf.TransactionDate.Year)
                    .Select(g => new CashFlowSummaryDto
                    {
                        TotalInflow = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                        TotalOutflow = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                        NetCashFlow = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        OperatingCashFlow = g.Where(cf => cf.Type == CashFlowType.Operating)
                                            .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        InvestingCashFlow = g.Where(cf => cf.Type == CashFlowType.Investing)
                                            .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        FinancingCashFlow = g.Where(cf => cf.Type == CashFlowType.Financing)
                                            .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                        PeriodStart = new DateTime(g.Key, 1, 1),
                        PeriodEnd = new DateTime(g.Key, 12, 31)
                    })
                    .OrderBy(t => t.PeriodStart)
                    .ToList();

                return yearlyTrends;
            }
        }

        public async Task<object> GetCashFlowTrendsAsync(Guid companyId, int months = 12)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);

            var inflows = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate))
                .ToList();

            var monthlyTrends = inflows
                .GroupBy(cf => new { cf.TransactionDate.Year, cf.TransactionDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalInflow = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    TotalOutflow = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetCashFlow = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    OperatingCashFlow = g.Where(cf => cf.Type == CashFlowType.Operating)
                                        .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    InvestingCashFlow = g.Where(cf => cf.Type == CashFlowType.Investing)
                                        .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    FinancingCashFlow = g.Where(cf => cf.Type == CashFlowType.Financing)
                                        .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount)
                })
                .OrderBy(t => t.Date)
                .ToList();

            return monthlyTrends;
        }

        #endregion

        #region Cash Flow Projections

        public async Task<object> GetCashFlowProjectionAsync(Guid companyId, DateTime projectionDate, int months = 12)
        {
            // Obtener datos históricos para calcular promedios
            var historicalMonths = 12;
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-historicalMonths);

            var historicalCashFlows = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate))
                .ToList();

            // Calcular promedios mensuales por tipo
            var monthlyAverages = historicalCashFlows
                .GroupBy(cf => new { cf.Type, cf.IsInflow })
                .Select(g => new
                {
                    Type = g.Key.Type,
                    IsInflow = g.Key.IsInflow,
                    MonthlyAverage = g.Sum(cf => cf.Amount) / historicalMonths
                })
                .ToList();

            // Generar proyecciones
            var projections = new List<object>();
            var currentBalance = await GetCurrentCashPositionAsync(companyId);
            var runningBalance = currentBalance;

            for (int i = 1; i <= months; i++)
            {
                var monthProjectionDate = projectionDate.AddMonths(i);
                var projectedInflow = monthlyAverages
                    .Where(ma => ma.IsInflow)
                    .Sum(ma => ma.MonthlyAverage);
                var projectedOutflow = monthlyAverages
                    .Where(ma => !ma.IsInflow)
                    .Sum(ma => ma.MonthlyAverage);
                var netProjection = projectedInflow - projectedOutflow;
                runningBalance += netProjection;

                projections.Add(new
                {
                    Month = monthProjectionDate.ToString("yyyy-MM"),
                    Date = monthProjectionDate,
                    ProjectedInflow = projectedInflow,
                    ProjectedOutflow = projectedOutflow,
                    NetProjection = netProjection,
                    ProjectedBalance = runningBalance
                });
            }

            return new
            {
                CurrentBalance = currentBalance,
                ProjectionPeriod = $"{months} months",
                Projections = projections
            };
        }

        #endregion

        #region Cash Flow Reports

        public async Task<object> GetCashFlowForecastAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var historicalData = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate.AddMonths(-12) && 
                                     cf.TransactionDate < startDate))
                .ToList();

            var monthlyAverages = historicalData
                .GroupBy(cf => new { cf.IsInflow, cf.Type })
                .Select(g => new
                {
                    IsInflow = g.Key.IsInflow,
                    Type = g.Key.Type,
                    MonthlyAverage = g.Sum(cf => cf.Amount) / 12
                })
                .ToList();

            var forecastPeriod = (endDate - startDate).Days / 30; // Approximate months
            var forecasts = new List<object>();
            var currentDate = startDate;

            for (int i = 0; i < forecastPeriod; i++)
            {
                var monthEnd = currentDate.AddMonths(1).AddDays(-1);
                if (monthEnd > endDate) monthEnd = endDate;

                var operatingInflow = monthlyAverages
                    .Where(ma => ma.IsInflow && ma.Type == CashFlowType.Operating)
                    .Sum(ma => ma.MonthlyAverage);
                var operatingOutflow = monthlyAverages
                    .Where(ma => !ma.IsInflow && ma.Type == CashFlowType.Operating)
                    .Sum(ma => ma.MonthlyAverage);
                var investingNet = monthlyAverages
                    .Where(ma => ma.Type == CashFlowType.Investing)
                    .Sum(ma => ma.IsInflow ? ma.MonthlyAverage : -ma.MonthlyAverage);
                var financingNet = monthlyAverages
                    .Where(ma => ma.Type == CashFlowType.Financing)
                    .Sum(ma => ma.IsInflow ? ma.MonthlyAverage : -ma.MonthlyAverage);

                forecasts.Add(new
                {
                    Period = currentDate.ToString("yyyy-MM"),
                    StartDate = currentDate,
                    EndDate = monthEnd,
                    ForecastedOperatingInflow = operatingInflow,
                    ForecastedOperatingOutflow = operatingOutflow,
                    ForecastedOperatingNet = operatingInflow - operatingOutflow,
                    ForecastedInvestingNet = investingNet,
                    ForecastedFinancingNet = financingNet,
                    ForecastedNetCashFlow = (operatingInflow - operatingOutflow) + investingNet + financingNet
                });

                currentDate = currentDate.AddMonths(1);
            }

            return new
            {
                ForecastPeriod = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                BasedOnHistoricalData = $"{startDate.AddMonths(-12):yyyy-MM-dd} to {startDate.AddDays(-1):yyyy-MM-dd}",
                Forecasts = forecasts
            };
        }

        public async Task<object> GetOperatingCashFlowReportAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var operatingCashFlows = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.Type == CashFlowType.Operating &&
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate))
                .ToList();

            var categoryBreakdown = operatingCashFlows
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(cb => Math.Abs(cb.NetAmount))
                .ToList();

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalOperatingInflows = operatingCashFlows.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                TotalOperatingOutflows = operatingCashFlows.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                NetOperatingCashFlow = operatingCashFlows.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                CategoryBreakdown = categoryBreakdown
            };
        }

        public async Task<object> GetInvestingCashFlowReportAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var investingCashFlows = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.Type == CashFlowType.Investing &&
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate))
                .ToList();

            var categoryBreakdown = investingCashFlows
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(cb => Math.Abs(cb.NetAmount))
                .ToList();

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalInvestingInflows = investingCashFlows.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                TotalInvestingOutflows = investingCashFlows.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                NetInvestingCashFlow = investingCashFlows.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                CategoryBreakdown = categoryBreakdown
            };
        }

        public async Task<object> GetFinancingCashFlowReportAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var financingCashFlows = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.Type == CashFlowType.Financing &&
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate))
                .ToList();

            var categoryBreakdown = financingCashFlows
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(cb => Math.Abs(cb.NetAmount))
                .ToList();

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalFinancingInflows = financingCashFlows.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                TotalFinancingOutflows = financingCashFlows.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                NetFinancingCashFlow = financingCashFlows.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                CategoryBreakdown = categoryBreakdown
            };
        }

        public async Task<object> GetCashFlowStatementAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            return await GenerateCashFlowStatementAsync(companyId, startDate, endDate);
        }

        public async Task<object> GenerateCashFlowStatementAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var outflows = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate))
                .ToList();

            var operatingActivities = outflows
                .Where(cf => cf.Type == CashFlowType.Operating)
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount)
                })
                .ToList();

            var investingActivities = outflows
                .Where(cf => cf.Type == CashFlowType.Investing)
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount)
                })
                .ToList();

            var financingActivities = outflows
                .Where(cf => cf.Type == CashFlowType.Financing)
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount)
                })
                .ToList();

            var totalOperating = operatingActivities.Sum(oa => oa.NetAmount);
            var totalInvesting = investingActivities.Sum(ia => ia.NetAmount);
            var totalFinancing = financingActivities.Sum(fa => fa.NetAmount);
            var netCashFlow = totalOperating + totalInvesting + totalFinancing;

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                OperatingActivities = new
                {
                    Activities = operatingActivities,
                    Total = totalOperating
                },
                InvestingActivities = new
                {
                    Activities = investingActivities,
                    Total = totalInvesting
                },
                FinancingActivities = new
                {
                    Activities = financingActivities,
                    Total = totalFinancing
                },
                NetCashFlow = netCashFlow
            };
        }

        #endregion

        #region Cash Flow Categories

        public async Task<IEnumerable<string>> GetCashFlowCategoriesAsync(Guid companyId)
        {
            var categories = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && !string.IsNullOrEmpty(cf.Category)))
                .Select(cf => cf.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return categories;
        }

        public async Task<IEnumerable<CashFlowResponseDto>> GetCashFlowsByCategoryAsync(Guid companyId, string category, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = (await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && cf.Category == category))
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(cf => cf.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(cf => cf.TransactionDate <= endDate.Value);

            var cashFlows = query
                .OrderByDescending(cf => cf.TransactionDate)
                .ToList();

            return cashFlows.Select(MapToCashFlowResponseDto);
        }

        public async Task<object> GetCashFlowByCategoryReportAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var cashFlowsQuery = await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate);
            var cashFlows = cashFlowsQuery.ToList();

            var categoryReport = cashFlows
                .GroupBy(cf => cf.Category ?? "Sin categoría")
                .Select(g => new
                {
                    Category = g.Key,
                    TotalInflow = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    TotalOutflow = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetAmount = g.Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    TransactionCount = g.Count(),
                    OperatingAmount = g.Where(cf => cf.Type == CashFlowType.Operating)
                                      .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    InvestingAmount = g.Where(cf => cf.Type == CashFlowType.Investing)
                                      .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount),
                    FinancingAmount = g.Where(cf => cf.Type == CashFlowType.Financing)
                                      .Sum(cf => cf.IsInflow ? cf.Amount : -cf.Amount)
                })
                .OrderByDescending(cr => Math.Abs(cr.NetAmount))
                .ToList();

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                Categories = categoryReport,
                Summary = new
                {
                    TotalCategories = categoryReport.Count,
                    TotalInflow = categoryReport.Sum(cr => cr.TotalInflow),
                    TotalOutflow = categoryReport.Sum(cr => cr.TotalOutflow),
                    NetCashFlow = categoryReport.Sum(cr => cr.NetAmount)
                }
            };
        }

        #endregion

        #region Cash Position

        public async Task<decimal> GetCurrentCashPositionAsync(Guid companyId)
        {
            // Sumar saldos de todas las cuentas bancarias activas
            var bankAccounts = await _bankAccountRepository
                .WhereAsync(ba => ba.CompanyId == companyId && ba.IsActive);
            var totalBalance = bankAccounts.Sum(ba => ba.Balance);

            return totalBalance;
        }

        public async Task<object> GetCashPositionHistoryAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var dailyPositions = new List<object>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var positionAtDate = await GetInitialCashPositionAsync(companyId, currentDate.AddDays(1));
                
                dailyPositions.Add(new
                {
                    Date = currentDate,
                    CashPosition = positionAtDate
                });

                currentDate = currentDate.AddDays(1);
            }

            var weeklyPositions = dailyPositions
                .GroupBy(dp => new { Year = ((DateTime)dp.GetType().GetProperty("Date").GetValue(dp)).Year, 
                                   Week = GetWeekOfYear((DateTime)dp.GetType().GetProperty("Date").GetValue(dp)) })
                .Select(g => new
                {
                    Week = $"{g.Key.Year}-W{g.Key.Week:00}",
                    StartDate = g.Min(dp => (DateTime)dp.GetType().GetProperty("Date").GetValue(dp)),
                    EndDate = g.Max(dp => (DateTime)dp.GetType().GetProperty("Date").GetValue(dp)),
                    AverageCashPosition = g.Average(dp => (decimal)dp.GetType().GetProperty("CashPosition").GetValue(dp)),
                    MinCashPosition = g.Min(dp => (decimal)dp.GetType().GetProperty("CashPosition").GetValue(dp)),
                    MaxCashPosition = g.Max(dp => (decimal)dp.GetType().GetProperty("CashPosition").GetValue(dp))
                })
                .ToList();

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                DailyPositions = dailyPositions,
                WeeklyPositions = weeklyPositions,
                Summary = new
                {
                    StartingPosition = dailyPositions.FirstOrDefault(),
                    EndingPosition = dailyPositions.LastOrDefault(),
                    AveragePosition = dailyPositions.Average(dp => (decimal)dp.GetType().GetProperty("CashPosition").GetValue(dp)),
                    MinPosition = dailyPositions.Min(dp => (decimal)dp.GetType().GetProperty("CashPosition").GetValue(dp)),
                    MaxPosition = dailyPositions.Max(dp => (decimal)dp.GetType().GetProperty("CashPosition").GetValue(dp))
                }
            };
        }

        public async Task<object> GetCashBurnRateAsync(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var cashFlowsQuery = await _cashFlowRepository
                .WhereAsync(cf => cf.CompanyId == companyId && 
                                     cf.TransactionDate >= startDate && 
                                     cf.TransactionDate <= endDate);
            var cashFlows = cashFlowsQuery.ToList();

            var totalDays = (endDate - startDate).Days;
            var totalMonths = totalDays / 30.0;

            var totalOutflows = cashFlows.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount);
            var totalInflows = cashFlows.Where(cf => cf.IsInflow).Sum(cf => cf.Amount);
            var netBurn = totalOutflows - totalInflows;

            var dailyBurnRate = totalDays > 0 ? netBurn / totalDays : 0;
            var monthlyBurnRate = totalMonths > 0 ? netBurn / (decimal)totalMonths : 0;

            var operatingOutflows = cashFlows
                .Where(cf => !cf.IsInflow && cf.Type == CashFlowType.Operating)
                .Sum(cf => cf.Amount);
            var operatingInflows = cashFlows
                .Where(cf => cf.IsInflow && cf.Type == CashFlowType.Operating)
                .Sum(cf => cf.Amount);
            var operatingBurnRate = totalMonths > 0 ? (operatingOutflows - operatingInflows) / (decimal)totalMonths : 0;

            var currentCashPosition = await GetCurrentCashPositionAsync(companyId);
            var runwayMonths = monthlyBurnRate > 0 ? currentCashPosition / monthlyBurnRate : decimal.MaxValue;

            var monthlyBreakdown = cashFlows
                .GroupBy(cf => new { cf.TransactionDate.Year, cf.TransactionDate.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:00}",
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Inflows = g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    Outflows = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount),
                    NetBurn = g.Where(cf => !cf.IsInflow).Sum(cf => cf.Amount) - g.Where(cf => cf.IsInflow).Sum(cf => cf.Amount),
                    OperatingInflows = g.Where(cf => cf.IsInflow && cf.Type == CashFlowType.Operating).Sum(cf => cf.Amount),
                    OperatingOutflows = g.Where(cf => !cf.IsInflow && cf.Type == CashFlowType.Operating).Sum(cf => cf.Amount),
                    OperatingBurn = g.Where(cf => !cf.IsInflow && cf.Type == CashFlowType.Operating).Sum(cf => cf.Amount) - 
                                   g.Where(cf => cf.IsInflow && cf.Type == CashFlowType.Operating).Sum(cf => cf.Amount)
                })
                .OrderBy(mb => mb.Year).ThenBy(mb => mb.Month)
                .ToList();

            return new
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalDays = totalDays,
                TotalMonths = Math.Round((decimal)totalMonths, 2),
                BurnRates = new
                {
                    DailyBurnRate = Math.Round(dailyBurnRate, 2),
                    MonthlyBurnRate = Math.Round(monthlyBurnRate, 2),
                    OperatingBurnRate = Math.Round(operatingBurnRate, 2)
                },
                CashPosition = new
                {
                    Current = currentCashPosition,
                    RunwayMonths = runwayMonths == decimal.MaxValue ? "Unlimited" : Math.Round(runwayMonths, 1).ToString()
                },
                Summary = new
                {
                    TotalInflows = totalInflows,
                    TotalOutflows = totalOutflows,
                    NetBurn = netBurn,
                    OperatingInflows = operatingInflows,
                    OperatingOutflows = operatingOutflows,
                    OperatingNetBurn = operatingOutflows - operatingInflows
                },
                MonthlyBreakdown = monthlyBreakdown
            };
        }

        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        #endregion

        #region Private Helper Methods

        private async Task<decimal> GetInitialCashPositionAsync(Guid companyId, DateTime asOfDate)
        {
            var bankAccounts = await _bankAccountRepository
                .WhereAsync(ba => ba.CompanyId == companyId && ba.CreatedAt <= asOfDate);

            decimal totalBalance = 0;

            foreach (var account in bankAccounts)
            {
                var transactions = await _bankTransactionRepository
                    .WhereAsync(bt => bt.BankAccountId == account.Id && bt.TransactionDate < asOfDate);
                var transactionSum = transactions.Sum(bt => bt.Type == BankTransactionType.Deposit ? bt.Amount : -bt.Amount);

                totalBalance += account.Balance + transactionSum;
            }

            return totalBalance;
        }

        private CashFlowResponseDto MapToCashFlowResponseDto(CashFlow cashFlow)
        {
            return new CashFlowResponseDto
            {
                Id = cashFlow.Id,
                Description = cashFlow.Description,
                Type = cashFlow.Type,
                TypeName = cashFlow.Type.ToString(),
                Amount = cashFlow.Amount,
                IsInflow = cashFlow.IsInflow,
                TransactionDate = cashFlow.TransactionDate,
                Category = cashFlow.Category,
                ReferenceNumber = cashFlow.ReferenceNumber,
                RelatedAccountId = cashFlow.RelatedAccountId,
                RelatedAccountName = cashFlow.RelatedAccount?.Name ?? string.Empty,
                RelatedBankAccountId = cashFlow.RelatedBankAccountId,
                RelatedBankAccountName = cashFlow.RelatedBankAccount?.AccountName ?? string.Empty,
                Notes = cashFlow.Notes,
                CreatedAt = cashFlow.CreatedAt,
                UpdatedAt = cashFlow.UpdatedAt
            };
        }

        #endregion
    }
}