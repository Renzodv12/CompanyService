namespace CompanyService.Core.Interfaces
{
    /// <summary>
    /// Interface for commands that invalidate cache when executed
    /// </summary>
    public interface IInvalidatesCache
    {
        /// <summary>
        /// Gets the cache patterns to invalidate
        /// </summary>
        string[] CachePatterns { get; }
    }
}

