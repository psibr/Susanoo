using System;
using System.Diagnostics.CodeAnalysis;
using Susanoo.Command;
using Susanoo.Exceptions;
using Susanoo.Processing;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Susanoo.Proxies
{
    /// <summary>
    /// A proxy for single result command processors that allows an interception action to be invoked when an execution exception occurs
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class NoResultSetExceptionInterceptionProxy<TFilter>
        : NoResultSetProxy<TFilter>
    {
        private readonly Action<SusanooExecutionException> _exceptionInterception;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetExceptionInterceptionProxy{TFilter}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="exceptionInterception">The exception interception.</param>
        public NoResultSetExceptionInterceptionProxy(INoResultCommandProcessor<TFilter> source, 
            Action<SusanooExecutionException> exceptionInterception)
            : base(source)
        {
            _exceptionInterception = exceptionInterception;
        }

        /// <summary>
        ///     Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="SusanooExecutionException">Exception interception failed.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override int ExecuteNonQuery(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            try
            {
                return Source.ExecuteNonQuery(databaseManager, executableCommandInfo);
            }
            catch (SusanooExecutionException ex)
            {
                try
                {
                    _exceptionInterception(ex);
                }
                catch (Exception exFault)
                {
                    throw new SusanooExecutionException("Exception interception failed.", 
                        new AggregateException(exFault, ex), ex.Info, ex.Timeout, ex.Parameters);
                }

                throw;
            }

        }

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>TReturn.</returns>
        /// <exception cref="SusanooExecutionException">Exception interception failed.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            try
            {
                return Source.ExecuteScalar<TReturn>(databaseManager, executableCommandInfo);
            }
            catch (SusanooExecutionException ex)
            {
                try
                {
                    _exceptionInterception(ex);
                }
                catch (Exception exFault)
                {
                    throw new SusanooExecutionException("Exception interception failed.",
                        new AggregateException(exFault, ex), ex.Info, ex.Timeout, ex.Parameters);
                }

                throw;
            }
        }

#if !NETFX40

        /// <summary>
        ///     Executes the non query asynchronously.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Int32&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Exception interception failed.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            try
            {
                return await Source.ExecuteNonQueryAsync(databaseManager, executableCommandInfo, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (SusanooExecutionException ex)
            {
                try
                {
                    _exceptionInterception(ex);
                }
                catch (Exception exFault)
                {
                    throw new SusanooExecutionException("Exception interception failed.",
                        new AggregateException(exFault, ex), ex.Info, ex.Timeout, ex.Parameters);
                }

                throw;
            }
        }

        /// <summary>
        ///     Executes the scalar asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TReturn&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Exception interception failed.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            try
            {
                return await Source.ExecuteScalarAsync<TReturn>(databaseManager, executableCommandInfo, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (SusanooExecutionException ex)
            {
                try
                {
                    _exceptionInterception(ex);
                }
                catch (Exception exFault)
                {
                    throw new SusanooExecutionException("Exception interception failed.",
                        new AggregateException(exFault, ex), ex.Info, ex.Timeout, ex.Parameters);
                }

                throw;
            }
        }
#endif

    }
}
