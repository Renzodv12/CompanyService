using StackExchange.Redis;

namespace CompanyService.Core.Interfaces
{
    public interface IRedisService
    {
        // Basic operations
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetAsync<T>(string key);
        Task<bool> KeyExistsAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<long> DeleteByPatternAsync(string pattern);
        
        // Hash operations
        Task<bool> SetHashAsync<T>(string key, string field, T value);
        Task<T?> GetHashAsync<T>(string key, string field);
        Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key);
        
        // List operations
        Task<bool> SetListAsync<T>(string key, IEnumerable<T> values, TimeSpan? expiry = null);
        Task<List<T?>> GetListAsync<T>(string key, long start = 0, long stop = -1);
        
        // Sorted Set operations
        Task<bool> SetSortedSetAsync<T>(string key, IEnumerable<(T value, double score)> items, TimeSpan? expiry = null);
        Task<List<T?>> GetSortedSetAsync<T>(string key, long start = 0, long stop = -1, Order order = Order.Ascending);
        
        // Utility operations
        Task<bool> IncrementAsync(string key, long value = 1);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
        Task<TimeSpan?> GetExpiryAsync(string key);
        Task<long> GetDatabaseSizeAsync();
        Task FlushDatabaseAsync();
    }
}
