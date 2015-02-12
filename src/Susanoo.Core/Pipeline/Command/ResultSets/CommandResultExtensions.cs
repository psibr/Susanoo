using Susanoo.Pipeline.Command.ResultSets.Processing;

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Extension methods for ICommandResultExpressions
    /// </summary>
    public static class CommandResultExtensions
    {
        /// <summary>
        /// Gets a matching processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="resultExpression">The result expression.</param>
        /// <returns>IResultMapper&lt;TResult&gt;.</returns>
        public static IResultMapper<TResult> GetProcessor<TFilter, TResult>(this
            ICommandResultExpressionCore<TFilter> resultExpression)
            where TResult : new()
        {
            return SingleResultSetCommandProcessor<TFilter, TResult>.BuildOrRegenResultMapper(
                resultExpression.ToSingleResult<TResult>());
        }
    }
}