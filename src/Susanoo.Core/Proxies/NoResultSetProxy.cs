using System;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Processing;

namespace Susanoo.Proxies
{
    /// <summary>
    /// A proxy for no result set command processors that allows transforms to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class NoResultSetProxy<TFilter>
        : NoResultCommandProcessorStructure<TFilter>,
            INoResultCommandProcessor<TFilter>
    {
        /// <summary>
        /// The source processor the proxy wraps
        /// </summary>
        protected readonly INoResultCommandProcessor<TFilter> Source;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetProxy{TFilter}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        protected NoResultSetProxy(INoResultCommandProcessor<TFilter> source)
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
        /// Gets the CommandBuilder information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        public override ICommandBuilderInfo<TFilter> CommandBuilderInfo =>
            Source.CommandBuilderInfo;

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
        /// <returns>INoResultCommandProcessor&lt;TFilter&gt;.</returns>
        public override INoResultCommandProcessor<TFilter> InterceptOrProxyWith(Func<INoResultCommandProcessor<TFilter>,
            INoResultCommandProcessor<TFilter>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }
    }
}
