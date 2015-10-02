using System;
using System.Diagnostics.CodeAnalysis;
using Susanoo.Command;
using Susanoo.Exceptions;
using Susanoo.Processing;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Susanoo.Proxies.ExceptionInterception
{
    /// <summary>
    /// A proxy for Multiple result command processors that allows an interception action to be invoked when an execution exception occurs
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class MultipleResultSetExceptionInterceptionProxy<TFilter> 
        : MultipleResultSetProxy<TFilter>
    {
        private readonly Action<SusanooExecutionException> _exceptionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetExceptionInterceptionProxy{TFilter}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        public MultipleResultSetExceptionInterceptionProxy(IMultipleResultSetCommandProcessor<TFilter> source,
             Action<SusanooExecutionException> exceptionHandler)
            : base(source)
        {
            _exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>IResultSetReader.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override IResultSetReader Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            try
            {
                return Source.Execute(databaseManager, executableCommandInfo);
            }
            catch (SusanooExecutionException ex)
            {
                try
                {
                    _exceptionHandler(ex);
                }
                catch (Exception exFault)
                {
                    throw new SusanooExecutionException("Exception interception failed.", new AggregateException(exFault, ex), ex.Info, ex.Timeout, ex.Parameters);
                }

                throw;
            }
        }

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override IMultipleResultSetCommandProcessor<TFilter> InterceptOrProxyWith(Func<IMultipleResultSetCommandProcessor<TFilter>, IMultipleResultSetCommandProcessor<TFilter>> interceptOrProxy)
        {
            return Source.InterceptOrProxyWith(interceptOrProxy)
                .InterceptOrProxyWith(s => 
                    new MultipleResultSetExceptionInterceptionProxy<TFilter>(s,
                        _exceptionHandler));
        }

#if !NETFX40
        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Exception interception failed.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override async Task<IResultSetReader> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            try
            {
                return await Source.ExecuteAsync(databaseManager, executableCommandInfo, cancellationToken);
            }
            catch (SusanooExecutionException ex)
            {
                try
                {
                    _exceptionHandler(ex);
                }
                catch (Exception exFault)
                {
                    throw new SusanooExecutionException("Exception interception failed.", new AggregateException(exFault, ex), ex.Info, ex.Timeout, ex.Parameters);
                }

                throw;
            }
        }
#endif
    }
}
