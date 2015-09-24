using Susanoo.Command;
using Susanoo.Pipeline;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Exposes information to CommandBuilder Processors for result mapping.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandResultInfo<in TFilter> :
        IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the command information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        ICommandBuilderInfo<TFilter> GetCommandInfo();

        /// <summary>
        /// Gets the result mapping exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExporter.</returns>
        ICommandResultMappingExporter GetExporter();
    }
}