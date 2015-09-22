using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// Describes a factory that can build a command processor that returns a single result set.
    /// </summary>
    public interface ISingleResultSetCommandProcessorFactory
    {
        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        ICommandProcessor<TFilter, TResult> BuildCommandProcessor<TFilter, TResult>(ICommandResultInfo<TFilter> mappings, string name = null);
    }
}