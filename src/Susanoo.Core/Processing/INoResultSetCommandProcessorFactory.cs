using Susanoo.Command;

namespace Susanoo.Processing
{
    /// <summary>
    /// Builds a command processor that returns a scalar values or return codes.
    /// </summary>
    public interface INoResultSetCommandProcessorFactory
    {
        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter&gt;.</returns>
        INoResultCommandProcessor<TFilter> BuildCommandProcessor<TFilter>(ICommandBuilderInfo<TFilter> command);
    }
}