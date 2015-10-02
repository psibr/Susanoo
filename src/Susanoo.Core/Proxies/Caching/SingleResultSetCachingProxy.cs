using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Susanoo.Command;
using Susanoo.Processing;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Susanoo.Proxies.Caching
{
    /// <summary>
    /// A proxy for single result command processors that allows Cachings to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class SingleResultSetCachingProxy<TFilter, TResult>
            : SingleResultSetProxy<TFilter, TResult>
    {
        private readonly ICacheProvider _cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetCachingProxy{TFilter,TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="cacheProvider">The cache provider.</param>
        public SingleResultSetCachingProxy(ISingleResultSetCommandProcessor<TFilter, TResult> source,
            ICacheProvider cacheProvider)
            : base(source)
        {
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public override IEnumerable<TResult> Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            var key = executableCommandInfo.GetDeterministicKey();
            IEnumerable<TResult> result = null;
            try
            {
                result = _cacheProvider.Get<IEnumerable<TResult>>(key);
            }
            catch (RuntimeBinderException) { }
            catch (SerializationException) { }

            if (result == null)
            {
                result = Source.Execute(databaseManager, executableCommandInfo);

                try
                {
                    _cacheProvider.Set(key, result);
                }
                catch (RuntimeBinderException) { }
                catch (SerializationException) { }
            }

            return result;
        }

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override ISingleResultSetCommandProcessor<TFilter, TResult> InterceptOrProxyWith(Func<ISingleResultSetCommandProcessor<TFilter, TResult>, ISingleResultSetCommandProcessor<TFilter, TResult>> interceptOrProxy)
        {
            return interceptOrProxy(this);
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
        public override async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            var key = executableCommandInfo.GetDeterministicKey();
            var result = _cacheProvider.Get<IEnumerable<TResult>>(key);

            if (result == null)
            {
                result = await Source.ExecuteAsync(databaseManager, executableCommandInfo, cancellationToken);

                _cacheProvider.Set(key, result);
            }

            return result;
        }
#endif
    }
}
