using System.Numerics;

namespace Susanoo
{
    /// <summary>
    /// A fragment or step in the Fluent Pipeline API.
    /// </summary>
    public interface IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        BigInteger CacheHash { get; }
    }
}