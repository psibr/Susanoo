using Susanoo.Exceptions;
using Susanoo.Processing;
using Susanoo.Proxies.Caching;
using Susanoo.Proxies.ExceptionInterception;
using Susanoo.Proxies.Transforms;
using System;
using System.Collections.Generic;

namespace Susanoo
{
    /// <summary>
    /// Provides extensions to the pipeline at the single result command processor just prior to execution.
    /// </summary>
    public static class SingleResultSetProxyExtensions
    {
        /// <summary>
        /// Applies a set of transforms to future executions.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">At least one transform is required.</exception>
        public static ISingleResultSetCommandProcessor<TFilter, TResult> ApplyTransforms<TFilter, TResult>(
            this ISingleResultSetCommandProcessor<TFilter, TResult> source, Func<ISingleResultSetCommandProcessor<TFilter, TResult>, IEnumerable<CommandTransform>> transforms)
        {
            if (transforms == null)
                throw new ArgumentNullException(nameof(transforms), "At least one transform is required.");

            return source.InterceptOrProxyWith(s => new SingleResultSetTransformProxy<TFilter, TResult>(s, transforms(s)));
        }

        /// <summary>
        /// Allows providing an action which is executed against any execution exceptions.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="executionHandler">The execution handler.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static ISingleResultSetCommandProcessor<TFilter, TResult> InterceptExceptions<TFilter, TResult>(
            this ISingleResultSetCommandProcessor<TFilter, TResult> source, Action<SusanooExecutionException> executionHandler)
        {
            if (executionHandler == null)
                throw new ArgumentNullException(nameof(executionHandler));

            return
                source.InterceptOrProxyWith(
                    s => new SingleResultSetExceptionInterceptionProxy<TFilter, TResult>(s, executionHandler));
        }

        /// <summary>
        /// Caches the results.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>ISingleResultSetCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">provider</exception>
        public static ISingleResultSetCommandProcessor<TFilter, TResult> CacheResults<TFilter, TResult>(
            this ISingleResultSetCommandProcessor<TFilter, TResult> source, ICacheProvider provider)
        {
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));

            return source
                .InterceptOrProxyWith(s =>
                    new SingleResultSetCachingProxy<TFilter, TResult>(s, provider));
        } 
    }
}