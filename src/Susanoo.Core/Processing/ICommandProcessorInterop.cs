using Susanoo.Command;
using Susanoo.Pipeline;

namespace Susanoo.Processing
{
    /// <summary>
    /// Shared members for all CommandBuilder processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandProcessorInterop<in TFilter> : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the CommandBuilder information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        ICommandBuilderInfo<TFilter> CommandBuilderInfo { get; }


    }
}