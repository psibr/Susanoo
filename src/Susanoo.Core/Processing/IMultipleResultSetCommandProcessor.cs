using System.Data.Common;
using Susanoo.Pipeline;

namespace Susanoo.Processing
{
    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface IMultipleResultSetCommandProcessor<in TFilter>
        : IFluentPipelineFragment
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>IMultipleResultSetCommandProcessor&lt;TFilter&gt;.</returns>
        IMultipleResultSetCommandProcessor<TFilter> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = default(double?));
        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IResultSetReader.</returns>
        IResultSetReader Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IResultSetReader.</returns>
        IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);
        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IResultSetReader.</returns>
        IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters);
    }
}