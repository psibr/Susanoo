using System.Data;

namespace Susanoo.Command
{
    /// <summary>
    /// Basic details about a CommandBuilder as defined by a CommandBuilder expression
    /// </summary>
    public interface ICommandInfo
    {
        /// <summary>
        /// Gets the CommandBuilder text.
        /// </summary>
        /// <value>The CommandBuilder text.</value>
        string CommandText { get; }

        /// <summary>
        /// Gets the type of the database CommandBuilder.
        /// </summary>
        /// <value>The type of the database CommandBuilder.</value>
        CommandType DbCommandType { get; }
    }
}