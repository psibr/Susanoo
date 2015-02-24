using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Susanoo.Pipeline.Command
{
    /// <summary>
    /// Basic details about a Command Expression
    /// </summary>
    public interface ICommandExpressionInfo : IFluentPipelineFragment
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

        /// <summary>
        /// Gets a value indicating whether storing column information is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow store column information]; otherwise, <c>false</c>.</value>
        bool AllowStoringColumnInfo { get; }

        /// <summary>
        /// Retrieves the where filter options.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        IDictionary<string, object> RetrieveWhereFilterOptions();
    }

    /// <summary>
    /// Basic details about a Command Expression and parameter building.
    /// </summary>
    public interface ICommandExpressionInfo<in TFilter>
        : ICommandExpressionInfo
    {
        /// <summary>
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        DbParameter[] BuildParameters(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters);
    }
}
