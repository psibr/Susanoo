using Susanoo.Processing;

namespace Susanoo.ResultSets
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
        /// <param name="resultInfo">The result information.</param>
        /// <returns>IResultMapper&lt;TResult&gt;.</returns>
        public static IResultMapper<TResult> GetProcessor<TFilter, TResult>(this
            ICommandResultInfo<TFilter> resultInfo)
        {
            return SingleResultSetCommandProcessor<TFilter, TResult>.BuildOrRegenResultMapper(
                resultInfo);
        }
    }
}