using Susanoo.Pipeline;
using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// Shared members for all CommandBuilder processors that have ResultSets.
    /// </summary>
    public interface ICommandProcessorWithResults<in TFilter> :
        ICommandProcessorWithResults
    {
        /// <summary>
        /// Gets the CommandBuilder result information.
        /// </summary>
        /// <value>The CommandBuilder result information.</value>
        ICommandResultInfo<TFilter> CommandResultInfo { get; }
    }

    /// <summary>
    /// Shared members for all CommandBuilder processors that have ResultSets.
    /// </summary>
    public interface ICommandProcessorWithResults : IFluentPipelineFragment
    {
        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        void ClearColumnIndexInfo();

        /// <summary>
        /// Flushes the result cache.
        /// </summary>
        void FlushCache();
    }
}