using CompanyService.Core.Models.Cache;
using Microsoft.Extensions.Configuration;

namespace CompanyService.Core.Services
{
    /// <summary>
    /// Helper class for cache operations with configuration support
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// Gets cache expiry from configuration or environment variable
        /// </summary>
        public static int GetCacheExpiryMinutes(IConfiguration configuration, string cacheKey, int defaultMinutes)
        {
            // Try configuration first, then environment variable, then default
            var configValue = configuration[$"Cache:{cacheKey}"];
            if (!string.IsNullOrEmpty(configValue))
            {
                return int.Parse(configValue);
            }

            var envVar = Environment.GetEnvironmentVariable($"CACHE_{cacheKey.ToUpper()}_EXPIRY_MINUTES");
            if (!string.IsNullOrEmpty(envVar))
            {
                return int.Parse(envVar);
            }

            return defaultMinutes;
        }

        /// <summary>
        /// Creates a cache policy with the specified expiry
        /// </summary>
        public static CachePolicy CreatePolicy(int expiryMinutes)
        {
            return new CachePolicy
            {
                Expiry = TimeSpan.FromMinutes(expiryMinutes)
            };
        }
    }
}
