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
        ICommandInfo<TFilter> Command { get; }

        /// <summary>
        /// Tries to add command modifier.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <returns><c>true</c> if no other modifier exists with the same priority, <c>false</c> otherwise.</returns>
        bool TryAddCommandModifier(CommandModifier modifier);
    }
}