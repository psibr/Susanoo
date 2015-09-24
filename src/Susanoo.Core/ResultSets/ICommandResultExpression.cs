using Susanoo.Command;
using Susanoo.Pipeline;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Shared components for CommandBuilder Result Expressions.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandResultExpression<in TFilter> :
        IFluentPipelineFragment
    {

        /// <summary>
        /// Gets the CommandBuilder.
        /// </summary>
        /// <value>The CommandBuilder.</value>
        ICommandBuilderInfo<TFilter> Command { get; }
    }
}