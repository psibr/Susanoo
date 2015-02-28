using System.Collections.Generic;

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

        /// <summary>
        /// Gets the command modifiers.
        /// </summary>
        /// <value>The command modifiers.</value>
        IEnumerable<CommandModifier> CommandModifiers { get; }


    }

    //public interface ICommandSingleResultInfo<in TFilter> :
    //    ICommandResultInfo<TFilter>
    //{
    //    /// <summary>
    //    /// Gets the where filter options. Null if no where filter.
    //    /// </summary>
    //    /// <value>The where filter options.</value>
    //    IDictionary<string, object> WhereFilterOptions { get; }
    //}
}