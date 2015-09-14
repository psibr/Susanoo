using Susanoo.Command;
using Susanoo.Pipeline;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Exposes information to CommandBuilder Processors for result mapping.
    /// </summary>
    /// <typeparam name="TFilter">The type of the t filter.</typeparam>
    public interface ICommandResultInfo<in TFilter> :
        IFluentPipelineFragment
    {
        /// <summary>
        /// Gets or sets the CommandBuilder information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        ICommandBuilderInfo<TFilter> GetCommandInfo();

        /// <summary>
        /// Converts to a single result expression.
        /// </summary>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultMappingExport GetExporter();
    }
}