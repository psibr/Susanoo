using System.Data;
using System.Data.Common;
using System.Numerics;

namespace Susanoo.Pipeline.Command
{
    /// <summary>
    /// Represents a ready to be executed command.
    /// </summary>
    public interface IExecutableCommandInfo
        : ICommandInfo
    {
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        DbParameter[] Parameters { get; }
    }

    /// <summary>
    /// Represents a ready to be executed command.
    /// </summary>
    public class ExecutableCommandInfo :
        IExecutableCommandInfo
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        public CommandType DbCommandType { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public DbParameter[] Parameters { get; set; }
    }
}