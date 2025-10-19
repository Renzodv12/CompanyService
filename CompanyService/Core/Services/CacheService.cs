using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Cache;
using StackExchange.Redis;

namespace CompanyService.Core.Services
{
    /// <summary>
    /// Servicio avanzado de cache con políticas y invalidación automática
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, Func<Task<T?>> fallback = null, CachePolicy? policy = null);
        Task<bool> SetAsync<T>(string key, T value, CachePolicy? policy = null);
        Task<bool> RemoveAsync(string key);
        Task<long> RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
        Task<CacheInfo> GetCacheInfoAsync(string key);
        Task<List<CacheInfo>> GetCacheInfoByPatternAsync(string pattern);
        Task InvalidateCompanyDataAsync(Guid companyId, string dataType);
        Task InvalidateUserDataAsync(Guid userId, string dataType);
        Task InvalidatePatternAsync(string pattern);
    }

    public class CacheService : ICacheService
    {
        private readonly IRedisService _redisService;
        private readonly ILogger<CacheService> _logger;
        private readonly CacheConfiguration _config;

        public CacheService(IRedisService redisService, ILogger<CacheService> logger, IConfiguration configuration)
        {
            _redisService = redisService;
            _logger = logger;
            _config = new CacheConfiguration
            {
                DefaultExpiry = TimeSpan.FromMinutes(30),
                MaxKeyLength = 200,
                EnableCompression = true,
                EnableMetrics = true
            };
        }

        public async Task<T?> GetAsync<T>(string key, Func<Task<T?>> fallback = null, CachePolicy? policy = null)
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                var cachedValue = await _redisService.GetAsync<T>(cacheKey);
                
                if (cachedValue != null)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", cacheKey);
                    return cachedValue;
                }

                _logger.LogDebug("Cache miss for key: {Key}", cacheKey);

                if (fallback != null)
                {
                    var value = await fallback();
                    if (value != null)
                    {
                        await SetAsync(cacheKey, value, policy);
                    }
                    return value;
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key: {Key}", key);
                return fallback != null ? await fallback() : default;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, CachePolicy? policy = null)
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                var expiry = policy?.Expiry ?? _config.DefaultExpiry;
                
                var result = await _redisService.SetAsync(cacheKey, value, expiry);
                
                if (result)
                {
                    _logger.LogDebug("Cached value for key: {Key} with expiry: {Expiry}", cacheKey, expiry);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
                return false;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                return await _redisService.DeleteAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
                return false;
            }
        }

        public async Task<long> RemoveByPatternAsync(string pattern)
        {
            try
            {
                var cachePattern = BuildCacheKey(pattern);
                return await _redisService.DeleteByPatternAsync(cachePattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
                return 0;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                return await _redisService.KeyExistsAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        public async Task<CacheInfo> GetCacheInfoAsync(string key)
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                var exists = await _redisService.KeyExistsAsync(cacheKey);
                var expiry = await _redisService.GetExpiryAsync(cacheKey);
                
                return new CacheInfo
                {
                    Key = cacheKey,
                    Exists = exists,
                    Expiry = expiry,
                    CreatedAt = DateTime.UtcNow // Redis doesn't provide creation time
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache info for key: {Key}", key);
                return new CacheInfo { Key = key, Exists = false };
            }
        }

        public async Task<List<CacheInfo>> GetCacheInfoByPatternAsync(string pattern)
        {
            try
            {
                var cachePattern = BuildCacheKey(pattern);
                var server = _redisService.GetType().GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_redisService) as IDatabase;
                
                if (server?.Multiplexer == null)
                    return new List<CacheInfo>();

                var redisServer = server.Multiplexer.GetServer(server.Multiplexer.GetEndPoints().First());
                var keys = redisServer.Keys(pattern: cachePattern);
                
                var cacheInfos = new List<CacheInfo>();
                foreach (var key in keys)
                {
                    var exists = await _redisService.KeyExistsAsync(key);
                    var expiry = await _redisService.GetExpiryAsync(key);
                    
                    cacheInfos.Add(new CacheInfo
                    {
                        Key = key,
                        Exists = exists,
                        Expiry = expiry,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                
                return cacheInfos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache info by pattern: {Pattern}", pattern);
                return new List<CacheInfo>();
            }
        }

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
                    await RemoveByPatternAsync(pattern);
                }

                _logger.LogInformation("Invalidated cache for company {CompanyId} and data type {DataType}", companyId, dataType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for company {CompanyId} and data type {DataType}", companyId, dataType);
            }
        }

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
                    await RemoveByPatternAsync(pattern);
                }

                _logger.LogInformation("Invalidated cache for user {UserId} and data type {DataType}", userId, dataType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for user {UserId} and data type {DataType}", userId, dataType);
            }
        }

        public async Task InvalidatePatternAsync(string pattern)
        {
            try
            {
                await RemoveByPatternAsync(pattern);
                _logger.LogDebug("Invalidated cache pattern: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache pattern: {Pattern}", pattern);
            }
        }

        private string BuildCacheKey(string key)
        {
            var prefix = "companyservice";
            var fullKey = $"{prefix}:{key}";
            
            if (fullKey.Length > _config.MaxKeyLength)
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(fullKey));
                return $"{prefix}:hash:{Convert.ToBase64String(hash)}";
            }
            
            return fullKey;
        }
    }

}
