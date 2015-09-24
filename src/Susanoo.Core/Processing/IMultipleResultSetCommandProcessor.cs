using System.Data.Common;
using Susanoo.Pipeline;

namespace Susanoo.Processing
{
    public interface IMultipleResultSetCommandProcessor<in TFilter>
        : IFluentPipelineFragment
    {
        /// <summary>
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>IMultipleResultSetCommandProcessor&lt;TFilter&gt;.</returns>
        IMultipleResultSetCommandProcessor<TFilter> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = default(double?));
        IResultSetReader Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
        IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);
        IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters);
    }
}