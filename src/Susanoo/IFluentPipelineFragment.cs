using System.Numerics;

namespace Susanoo
{
    /// <summary>
    /// Indicates that a class is relevant to the fluent API.
    /// </summary>
    public interface IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the hash code used for caching.
        /// </summary>
        /// <value>hashcode</value>
        BigInteger CacheHash { get; }
    }
}