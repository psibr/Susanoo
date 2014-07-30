using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// A fully built and ready to be executed command expression with a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class NoResultSetCommandProcessor<TFilter> : ICommandProcessor<TFilter>, IFluentPipelineFragment
    {
        private readonly ICommandExpression<TFilter> _CommandExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetCommandProcessor{TFilter}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public NoResultSetCommandProcessor(ICommandExpression<TFilter> command)
        {
            this._CommandExpression = command;
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return _CommandExpression.CacheHash; }
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return databaseManager.ExecuteScalar<TReturn>(
                CommandExpression.CommandText,
                CommandExpression.DBCommandType,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return ExecuteScalar<TReturn>(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return databaseManager.ExecuteStoredProcedureNonQuery(
                CommandExpression.CommandText,
                CommandExpression.DBCommandType,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return ExecuteNonQuery(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteScalarAsync<TReturn>(CommandExpression.CommandText, CommandExpression.DBCommandType, default(CancellationToken), explicitParameters);
        }
    }
}