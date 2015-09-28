using System;
using Susanoo.Command;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Allows building of ICommandSingleResultExpressions.
    /// </summary>
    public interface ICommandSingleResultExpressionFactory
    {
        /// <summary>
        /// Builds the command Single result expression.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter&gt;.</returns>
        ICommandSingleResultExpression<TFilter, TResult> BuildCommandSingleResultExpression<TFilter, TResult>(
            ICommandBuilderInfo<TFilter> command);
    }
}