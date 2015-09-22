using System.Data;
using System.Data.Common;

namespace Susanoo.Command
{
    /// <summary>
    /// Represents a ready to be executed CommandBuilder.
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
    /// Represents a ready to be executed CommandBuilder.
    /// </summary>
    public class ExecutableCommandInfo :
        IExecutableCommandInfo
    {
        /// <summary>
        /// Gets the CommandBuilder text.
        /// </summary>
        /// <value>The CommandBuilder text.</value>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets the type of the database CommandBuilder.
        /// </summary>
        /// <value>The type of the database CommandBuilder.</value>
        public CommandType DbCommandType { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public DbParameter[] Parameters { get; set; }
    }
}