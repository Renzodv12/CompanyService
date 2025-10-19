namespace CompanyService.Core.Models.Cache
{
    /// <summary>
    /// Configuración del sistema de cache
    /// </summary>
    public class CacheConfiguration
    {
        public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(30);
        public int MaxKeyLength { get; set; } = 200;
        public bool EnableCompression { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
    }

    /// <summary>
    /// Política de cache
    /// </summary>
    public class CachePolicy
    {
        public TimeSpan? Expiry { get; set; }
        public bool SlidingExpiry { get; set; } = false;
        public CachePriority Priority { get; set; } = CachePriority.Normal;
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Prioridad del cache
    /// </summary>
    public enum CachePriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }

    /// <summary>
    /// Información del cache
    /// </summary>
    public class CacheInfo
    {
        public string Key { get; set; } = string.Empty;
        public bool Exists { get; set; }
        public TimeSpan? Expiry { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Estadísticas del cache
    /// </summary>
    public class CacheStatistics
    {
        public long TotalKeys { get; set; }
        public long HitCount { get; set; }
        public long MissCount { get; set; }
        public double HitRatio => TotalRequests > 0 ? (double)HitCount / TotalRequests : 0;
        public long TotalRequests => HitCount + MissCount;
        public long MemoryUsage { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Métricas de rendimiento del cache
    /// </summary>
    public class CacheMetrics
    {
        public TimeSpan AverageResponseTime { get; set; }
        public long RequestsPerSecond { get; set; }
        public long ErrorsPerSecond { get; set; }
        public Dictionary<string, long> KeyAccessCounts { get; set; } = new();
        public Dictionary<string, TimeSpan> KeyResponseTimes { get; set; } = new();
    }
}
