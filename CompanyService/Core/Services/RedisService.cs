using CompanyService.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CompanyService.Core.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
        {
            _database = redis.GetDatabase();
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                return await _database.StringSetAsync(key, serializedValue, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting key {Key} in Redis", key);
                return false;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                return value.HasValue ? JsonSerializer.Deserialize<T>(value!, _jsonOptions) : default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting key {Key} from Redis", key);
                return default;
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key {Key} exists in Redis", key);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                return await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting key {Key} from Redis", key);
                return false;
            }
        }

        public async Task<long> DeleteByPatternAsync(string pattern)
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                
                if (!keys.Any())
                    return 0;

                return await _database.KeyDeleteAsync(keys.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting keys by pattern {Pattern} from Redis", pattern);
                return 0;
            }
        }

        public async Task<bool> SetHashAsync<T>(string key, string field, T value)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                return await _database.HashSetAsync(key, field, serializedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting hash field {Field} for key {Key} in Redis", field, key);
                return false;
            }
        }

        public async Task<T?> GetHashAsync<T>(string key, string field)
        {
            try
            {
                var value = await _database.HashGetAsync(key, field);
                return value.HasValue ? JsonSerializer.Deserialize<T>(value!, _jsonOptions) : default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hash field {Field} for key {Key} from Redis", field, key);
                return default;
            }
        }

        public async Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key)
        {
            try
            {
                var hashFields = await _database.HashGetAllAsync(key);
                var result = new Dictionary<string, T?>();
                
                foreach (var field in hashFields)
                {
                    result[field.Name] = JsonSerializer.Deserialize<T>(field.Value!, _jsonOptions);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all hash fields for key {Key} from Redis", key);
                return new Dictionary<string, T?>();
            }
        }

        public async Task<bool> SetListAsync<T>(string key, IEnumerable<T> values, TimeSpan? expiry = null)
        {
            try
            {
                var serializedValues = values.Select(v => (RedisValue)JsonSerializer.Serialize(v, _jsonOptions)).ToArray();
                await _database.ListRightPushAsync(key, serializedValues);
                
                if (expiry.HasValue)
                {
                    await _database.KeyExpireAsync(key, expiry.Value);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting list for key {Key} in Redis", key);
                return false;
            }
        }

        public async Task<List<T?>> GetListAsync<T>(string key, long start = 0, long stop = -1)
        {
            try
            {
                var values = await _database.ListRangeAsync(key, start, stop);
                return values.Select(v => JsonSerializer.Deserialize<T>(v!, _jsonOptions)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting list for key {Key} from Redis", key);
                return new List<T?>();
            }
        }

        public async Task<bool> SetSortedSetAsync<T>(string key, IEnumerable<(T value, double score)> items, TimeSpan? expiry = null)
        {
            try
            {
                var sortedSetEntries = items.Select(item => 
                    new SortedSetEntry(JsonSerializer.Serialize(item.value, _jsonOptions), item.score)).ToArray();
                
                await _database.SortedSetAddAsync(key, sortedSetEntries);
                
                if (expiry.HasValue)
                {
                    await _database.KeyExpireAsync(key, expiry.Value);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting sorted set for key {Key} in Redis", key);
                return false;
            }
        }

        public async Task<List<T?>> GetSortedSetAsync<T>(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            try
            {
                var values = await _database.SortedSetRangeByRankAsync(key, start, stop, order);
                return values.Select(v => JsonSerializer.Deserialize<T>(v!, _jsonOptions)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sorted set for key {Key} from Redis", key);
                return new List<T?>();
            }
        }

        public async Task<bool> IncrementAsync(string key, long value = 1)
        {
            try
            {
                await _database.StringIncrementAsync(key, value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing key {Key} in Redis", key);
                return false;
            }
        }

        public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
        {
            try
            {
                return await _database.KeyExpireAsync(key, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting expiry for key {Key} in Redis", key);
                return false;
            }
        }

        public async Task<TimeSpan?> GetExpiryAsync(string key)
        {
            try
            {
                return await _database.KeyTimeToLiveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiry for key {Key} from Redis", key);
                return null;
            }
        }

        public async Task<long> GetDatabaseSizeAsync()
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                return await server.DatabaseSizeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database size from Redis");
                return 0;
            }
        }

        public async Task FlushDatabaseAsync()
        {
            try
            {
                await _database.ExecuteAsync("FLUSHDB");
                _logger.LogInformation("Redis database flushed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flushing Redis database");
            }
        }
    }
}
