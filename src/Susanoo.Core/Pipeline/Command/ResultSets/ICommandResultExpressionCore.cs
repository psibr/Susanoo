namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Shared components for Command Result Expressions.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandResultExpressionCore<in TFilter> :
        IFluentPipelineFragment
    {

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        ICommand<TFilter> Command { get; }
    }
}