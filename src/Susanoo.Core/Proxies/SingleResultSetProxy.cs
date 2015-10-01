using System;
using System.Numerics;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Proxies
{
    /// <summary>
    /// A proxy for single result command processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class SingleResultSetProxy<TFilter, TResult> 
        : SingleResultSetCommandProcessorStructure<TFilter, TResult>,
            ISingleResultSetCommandProcessor<TFilter, TResult>
    {
        /// <summary>
        /// The source processor the proxy wraps
        /// </summary>
        protected readonly ISingleResultSetCommandProcessor<TFilter, TResult> Source;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetProxy{TFilter,TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        protected SingleResultSetProxy(ISingleResultSetCommandProcessor<TFilter, TResult> source)
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
    }
}
