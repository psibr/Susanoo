#region

using Susanoo.Command;
using System.Data;

#endregion

namespace Susanoo.Pipeline
{
    /// <summary>
    /// Provides an entry point to defining commands and therein entering the Susanoo command Fluent API.
    /// </summary>
    public class CommandBuilder : ICommandBuilder
    {
        private readonly ICommandExpressionFactory _commandExpressionFactory;

        /// <summary>
        /// Resolves dependency on CommandExpressionFactory and instantiates.
        /// </summary>
        /// <param name="commandExpressionFactory"></param>
        public CommandBuilder(ICommandExpressionFactory commandExpressionFactory)
        {
            _commandExpressionFactory = commandExpressionFactory;
        }

        /// <summary>
        /// Begins the CommandBuilder definition process using a Fluent API implementation, move to next step with DefineResults on
        /// the result of this call.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual ICommandExpression<TFilter> DefineCommand<TFilter>(string commandText, CommandType commandType)
        {
            return _commandExpressionFactory
                .BuildCommandExpression<TFilter>(commandText, commandType);
        }

        /// <summary>
        /// Begins the CommandBuilder definition process using a Fluent API implementation, move to next step with DefineResults on
        /// the result of this call.
        /// </summary>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType)
        {
            return _commandExpressionFactory
                .BuildCommandExpression<dynamic>(commandText, commandType);
        }
    }
}