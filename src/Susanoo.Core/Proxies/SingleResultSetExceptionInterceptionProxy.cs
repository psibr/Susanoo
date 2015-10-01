using System;
using System.Collections.Generic;
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
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class SingleResultSetExceptionInterceptionProxy<TFilter, TResult> 
        : SingleResultSetProxy<TFilter, TResult>
    {
        private readonly Action<SusanooExecutionException> _exceptionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetExceptionInterceptionProxy{TFilter,TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        public SingleResultSetExceptionInterceptionProxy(ISingleResultSetCommandProcessor<TFilter, TResult> source,
             Action<SusanooExecutionException> exceptionHandler)
            : base(source)
        {
            _exceptionHandler = exceptionHandler;
        }
        
        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Exception interception failed.</exception>
        [SuppressMessage("ReSharper", "ThrowFromCatchWithNoInnerException")]
        public override IEnumerable<TResult> Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
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
        public override ISingleResultSetCommandProcessor<TFilter, TResult> InterceptOrProxyWith(Func<ISingleResultSetCommandProcessor<TFilter, TResult>, ISingleResultSetCommandProcessor<TFilter, TResult>> interceptOrProxy)
        {
            return Source.InterceptOrProxyWith(interceptOrProxy)
                .InterceptOrProxyWith(s => 
                    new SingleResultSetExceptionInterceptionProxy<TFilter, TResult>(s,
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
        public override async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            try
            {
                return await Source.ExecuteAsync(databaseManager, executableCommandInfo, cancellationToken)
                    .ConfigureAwait(false);
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
