using System.Numerics;

namespace Susanoo.Pipeline.Command
{
    /// <summary>
    /// Basic details about a Command and parameter building and access to change command text.
    /// </summary>
    /// <typeparam name="TFilter">The type of the t filter.</typeparam>
    public interface ICommand<in TFilter>
        : ICommandInfo<TFilter>
    {
        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        new string CommandText { get; set; }

        /// <summary>
        /// Recomputes the cache hash.
        /// </summary>
        /// <returns>BigInteger.</returns>
        BigInteger RecomputeCacheHash();
    }
}