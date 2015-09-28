using System;
using Susanoo.Command;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Allows building of ICommandMultipleResultExpressions.
    /// </summary>
    public interface ICommandMultipleResultExpressionFactory
    {
        /// <summary>
        /// Builds the command multiple result expression.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="resultTypes">The result types.</param>
        /// <returns>ICommandMultipleResultExpression&lt;TFilter&gt;.</returns>
        ICommandMultipleResultExpression<TFilter> BuildCommandMultipleResultExpression<TFilter>(ICommandBuilderInfo<TFilter> command, params Type[] resultTypes);
    }
}