#region

using System.Data;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Provides an entry point to defining commands and therein entering the Susanoo command Fluent API.
    /// </summary>
    public class CommandBuilder : ICommandExpressionBuilder
    {
        /// <summary>
        ///     Begins the command definition process using a Fluent API implementation, move to next step with DefineResults on
        ///     the result of this call.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        /// <exception cref="System.ArgumentException">
        ///     No command text provided.;commandText
        ///     or
        ///     TableDirect is not supported.;commandType
        /// </exception>
        public virtual ICommandExpression<TFilter> DefineCommand<TFilter>(string commandText, CommandType commandType)
        {
            return new CommandExpression<TFilter>(commandText, commandType);
        }

        /// <summary>
        ///     Begins the command definition process using a Fluent API implementation, move to next step with DefineResults on
        ///     the result of this call.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        /// <exception cref="System.ArgumentException">
        ///     No command text provided.;commandText
        ///     or
        ///     TableDirect is not supported.;commandType
        /// </exception>
        public virtual ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType)
        {
            return new CommandExpression<dynamic>(commandText, commandType);
        }
    }
}