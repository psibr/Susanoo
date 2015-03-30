using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Susanoo.Pipeline.Command
{
    /// <summary>
    /// Basic details about a Command as defined by a command expression
    /// </summary>
    public interface ICommandInfo
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        string CommandText { get; }

        /// <summary>
        /// Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        CommandType DbCommandType { get; }
    }

    /// <summary>
    /// Basic details about a Command and parameter building.
    /// </summary>
    public interface ICommandInfo<in TFilter>
        : ICommandInfo, IFluentPipelineFragment
    {
        /// <summary>
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">Additional parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        DbParameter[] BuildParameters(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Gets a value indicating whether storing column information is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow store column information]; otherwise, <c>false</c>.</value>
        bool AllowStoringColumnInfo { get; }

        /// <summary>
        /// Tries to add a command modifier.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <returns><c>true</c> if no other modifier exists with the same priority, <c>false</c> otherwise.</returns>
        bool TryAddCommandModifier(CommandModifier modifier);

        /// <summary>
        /// Adds or replaces a command modifier.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        void AddOrReplaceCommandModifier(CommandModifier modifier);

        /// <summary>
        /// Gets the command modifiers.
        /// </summary>
        /// <value>The command modifiers.</value>
        IEnumerable<CommandModifier> CommandModifiers { get; }

        /// <summary>
        /// Adds the query wrapper.
        /// </summary>
        /// <param name="additionalColumns">The additional columns.</param>
        void AddQueryWrapper(string additionalColumns = null);
    }
}