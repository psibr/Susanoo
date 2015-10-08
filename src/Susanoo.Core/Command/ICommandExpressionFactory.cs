using System.Data;

namespace Susanoo.Command
{
    /// <summary>
    /// Allows construction of a command expression
    /// </summary>
    public interface ICommandExpressionFactory
    {
        /// <summary>
        /// Builds the command expression.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        ICommandExpression<TFilter> BuildCommandExpression<TFilter>(string commandText, CommandType commandType);
    }
}