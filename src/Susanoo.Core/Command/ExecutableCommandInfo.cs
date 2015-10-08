using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Susanoo.Command
{
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

        /// <summary>
        /// Gets a deterministic key based on hashing.
        /// </summary>
        /// <returns>Base64 encoded 128bit hash code</returns>
        public string GetDeterministicKey()
        {
            return Convert.ToBase64String(HashBuilder.Compute(Parameters.Aggregate(CommandText + DbCommandType,
                (s, parameter) => s += parameter.ParameterName + parameter.DbType + parameter.Value.ToString())).ToByteArray());
        }
    }
}