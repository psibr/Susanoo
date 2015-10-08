using System.Collections.Generic;
using System.Data.Common;

namespace Susanoo.Command
{
    /// <summary>
    /// Basic details about a CommandBuilder and parameter building.
    /// </summary>
    public interface ICommandBuilderInfo<in TFilter>
        : ICommandInfo
    {
        /// <summary>
        /// Builds the parameters.
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
        /// Gets the property whitelist.
        /// </summary>
        /// <value>The whitelist.</value>
        IEnumerable<string> PropertyWhitelist { get; }

        /// <summary>
        /// Gets the property blacklist.
        /// </summary>
        /// <value>The blacklist.</value>
        IEnumerable<string> PropertyBlacklist { get; }
    }
}