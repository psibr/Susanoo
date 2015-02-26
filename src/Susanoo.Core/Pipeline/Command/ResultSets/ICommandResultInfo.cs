namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Exposes information to Command Processors for result mapping.
    /// </summary>
    /// <typeparam name="TFilter">The type of the t filter.</typeparam>
    public interface ICommandResultInfo<in TFilter> :
        IFluentPipelineFragment
    {
        /// <summary>
        /// Gets or sets the command information.
        /// </summary>
        /// <value>The command information.</value>
        ICommandInfo<TFilter> GetCommandInfo();

        /// <summary>
        /// Converts to a single result expression.
        /// </summary>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultMappingExport GetExporter();
    }
}