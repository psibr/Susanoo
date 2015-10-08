using Susanoo.Command;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Shared components for CommandBuilder Result Expressions.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandResultExpression<in TFilter>
    {

        /// <summary>
        /// Gets the CommandBuilder.
        /// </summary>
        /// <value>The CommandBuilder.</value>
        ICommandBuilderInfo<TFilter> Command { get; }
    }
}