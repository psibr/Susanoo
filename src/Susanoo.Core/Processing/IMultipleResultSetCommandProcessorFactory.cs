using Susanoo.ResultSets;
using System;

namespace Susanoo.Processing
{
    /// <summary>
    /// Builds a command processor that returns multiple result sets.
    /// </summary>
    public interface IMultipleResultSetCommandProcessorFactory
    {
        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="resultTypes">The result types.</param>
        /// <returns>IMultipleResultSetCommandProcessor&lt;TFilter&gt;.</returns>
        IMultipleResultSetCommandProcessor<TFilter> BuildCommandProcessor<TFilter>(ICommandResultInfo<TFilter> mappings,
            params Type[] resultTypes);
    }
}