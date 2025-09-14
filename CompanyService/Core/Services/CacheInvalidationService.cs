using CompanyService.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CompanyService.Core.Services
{
    /// <summary>
    /// Servicio para invalidar caché automáticamente cuando hay cambios en los datos
    /// </summary>
    public interface ICacheInvalidationService
    {
        Task InvalidateCompanyDataAsync(Guid companyId, string dataType);
        Task InvalidateUserDataAsync(Guid userId, string dataType);
        Task InvalidatePatternAsync(string pattern);
        Task InvalidateAllCompanyDataAsync(Guid companyId);
    }

    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly IRedisService _redisService;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(IRedisService redisService, ILogger<CacheInvalidationService> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        /// <summary>
        /// Invalida caché específico de una empresa y tipo de dato
        /// </summary>
        public async Task InvalidateCompanyDataAsync(Guid companyId, string dataType)
        {
            try
            {
                var patterns = new List<string>
                {
                    $"*:company:{companyId}*",
                    $"*{dataType}*:company:{companyId}*",
                    $"{dataType}*:company:{companyId}*"
                };

                foreach (var pattern in patterns)
                {
                    await InvalidatePatternAsync(pattern);
                }

                _logger.LogInformation("Invalidated cache for company {CompanyId} and data type {DataType}", companyId, dataType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for company {CompanyId} and data type {DataType}", companyId, dataType);
            }
        }

        /// <summary>
        /// Invalida caché específico de un usuario y tipo de dato
        /// </summary>
        public async Task InvalidateUserDataAsync(Guid userId, string dataType)
        {
            try
            {
                var patterns = new List<string>
                {
                    $"*:user:{userId}*",
                    $"*{dataType}*:user:{userId}*",
                    $"{dataType}*:user:{userId}*"
                };

                foreach (var pattern in patterns)
                {
                    await InvalidatePatternAsync(pattern);
                }

                _logger.LogInformation("Invalidated cache for user {UserId} and data type {DataType}", userId, dataType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for user {UserId} and data type {DataType}", userId, dataType);
            }
        }

        /// <summary>
        /// Invalida caché usando un patrón específico
        /// </summary>
        public async Task InvalidatePatternAsync(string pattern)
        {
            try
            {
                await _redisService.DeleteByPatternAsync(pattern);
                _logger.LogDebug("Invalidated cache pattern: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache pattern: {Pattern}", pattern);
            }
        }

        /// <summary>
        /// Invalida todo el caché relacionado con una empresa
        /// </summary>
        public async Task InvalidateAllCompanyDataAsync(Guid companyId)
        {
            try
            {
                await InvalidatePatternAsync($"*:company:{companyId}*");
                _logger.LogInformation("Invalidated all cache for company {CompanyId}", companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating all cache for company {CompanyId}", companyId);
            }
        }
    }

    /// <summary>
    /// Constantes para tipos de datos cacheables
    /// </summary>
    public static class CacheDataTypes
    {
        public const string Leads = "leads";
        public const string Opportunities = "opportunities";
        public const string Branches = "branches";
        public const string AccountsReceivable = "accounts-receivable";
        public const string BankAccounts = "bank-accounts";
        public const string CashFlows = "cash-flows";
        public const string Customers = "customers";
        public const string Departments = "departments";
        public const string CompanySettings = "company-settings";
        public const string CompanyDocuments = "company-documents";
        public const string Dashboard = "dashboard";
        public const string AuditLog = "audit-log";
        public const string Tasks = "tasks";
        public const string TaskBoard = "task-board";
        public const string Companies = "companies";
        public const string Menu = "menu";
    }
}