#region

using System;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif
using Susanoo.Command;
using Susanoo.Exceptions;

#endregion

namespace Susanoo.Processing
{
    /// <summary>
    ///     A fully built and ready to be executed CommandBuilder expression with a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public sealed class NoResultCommandProcessor<TFilter>
        : NoResultCommandProcessorStructure<TFilter>,
            INoResultCommandProcessor<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NoResultCommandProcessor{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public NoResultCommandProcessor(ICommandBuilderInfo<TFilter> command)
        {
            CommandBuilderInfo = command;
        }

        /// <summary>
        ///     Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Int32.</returns>
        public override int ExecuteNonQuery(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            int result;

            try
            {
                result = databaseManager.ExecuteNonQuery(
                    executableCommandInfo.CommandText,
                    executableCommandInfo.DbCommandType,
                    Timeout,
                    executableCommandInfo.Parameters);
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            return result;
        }

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>TReturn.</returns>
        public override TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            var result = default(TReturn);

            try
            {
                result = databaseManager.ExecuteScalar<TReturn>(
                    executableCommandInfo.CommandText,
                    executableCommandInfo.DbCommandType,
                    Timeout,
                    executableCommandInfo.Parameters);
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            return result;
        }

        /// <summary>
        ///     Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter&gt;.</returns>
        public override INoResultCommandProcessor<TFilter> InterceptOrProxyWith(
            Func<INoResultCommandProcessor<TFilter>, INoResultCommandProcessor<TFilter>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }

#if !NETFX40

        /// <summary>
        ///     Executes the non query asynchronously.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Int32&gt;.</returns>
        public override async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            var result = 0;

            try
            {
                result = await databaseManager.ExecuteNonQueryAsync(
                    executableCommandInfo.CommandText,
                    executableCommandInfo.DbCommandType,
                    Timeout,
                    cancellationToken,
                    executableCommandInfo.Parameters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            return result;
        }

        /// <summary>
        ///     Executes the scalar asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TReturn&gt;.</returns>
        public override async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            var result = default(TReturn);

            try
            {
                result = await databaseManager.ExecuteScalarAsync<TReturn>(
                    executableCommandInfo.CommandText,
                    executableCommandInfo.DbCommandType,
                    Timeout,
                    cancellationToken,
                    executableCommandInfo.Parameters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            return result;
        }


#endif
    }
}