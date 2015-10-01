using System;
using System.Numerics;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Proxies
{
    /// <summary>
    /// A proxy for multiple result command processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class MultipleResultSetProxy<TFilter> 
        : MultipleResultSetCommandProcessorStructure<TFilter>,
            IMultipleResultSetCommandProcessor<TFilter>
    {
        /// <summary>
        /// The source command processor the proxy wraps
        /// </summary>
        protected readonly IMultipleResultSetCommandProcessor<TFilter> Source;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetProxy{TFilter}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        protected MultipleResultSetProxy(IMultipleResultSetCommandProcessor<TFilter> source)
        {
            Source = source;
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash =>
            Source.CacheHash;

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        public override void ClearColumnIndexInfo()
        {
            Source.ClearColumnIndexInfo();
        }

        /// <summary>
        /// Gets the CommandBuilder result information.
        /// </summary>
        /// <value>The CommandBuilder result information.</value>
        public override ICommandResultInfo<TFilter> CommandResultInfo =>
            Source.CommandResultInfo;

        /// <summary>
        /// Gets or sets the timeout of a command execution.
        /// </summary>
        /// <value>The timeout.</value>
        public override TimeSpan Timeout
        {
            get { return Source.Timeout; }
            set { Source.Timeout = value; }
        }

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override IMultipleResultSetCommandProcessor<TFilter> InterceptOrProxyWith(Func<IMultipleResultSetCommandProcessor<TFilter>, IMultipleResultSetCommandProcessor<TFilter>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }
    }
}
