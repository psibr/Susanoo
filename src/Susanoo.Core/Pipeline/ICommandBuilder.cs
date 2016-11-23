#region

using Susanoo.Command;
using System.Data;

#endregion

namespace Susanoo.Pipeline
{
    /// <summary>
    /// Provides an entry point to defining commands and therein entering the Susanoo CommandBuilder Fluent API.
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        /// Begins the CommandBuilder definition process using a Fluent API implementation, move to next step with DefineResults on
        /// the result of this call.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandExpression<TFilter> DefineCommand<TFilter>(string commandText, CommandType commandType = CommandType.Text);

        /// <summary>
        /// Begins the CommandBuilder definition process using a Fluent API implementation, move to next step with DefineResults on
        /// the result of this call.
        /// </summary>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType = CommandType.Text);
    }
}