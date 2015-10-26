using Susanoo.Exceptions;
using Susanoo.Processing;
using Susanoo.Proxies.ExceptionInterception;
using Susanoo.Proxies.Transforms;
using System;
using System.Collections.Generic;

namespace Susanoo
{
    /// <summary>
    /// Provides extensions to the pipeline at the no result set command processor just prior to execution.
    /// </summary>
    public static class NoResultSetProxyExtensions
    {
        /// <summary>
        /// Applies a set of transforms to future executions.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="ArgumentNullException">At least one transform is required.</exception>
        public static INoResultCommandProcessor<TFilter> ApplyTransforms<TFilter>(
            this INoResultCommandProcessor<TFilter> source, Func<INoResultCommandProcessor<TFilter>, IEnumerable<CommandTransform>> transforms)
        {
            if (transforms == null)
                throw new ArgumentNullException(nameof(transforms), "At least one transform is required.");

            return source.InterceptOrProxyWith(s => new NoResultSetTransformProxy<TFilter>(s, transforms(s)));
        }

        /// <summary>
        /// Allows providing an action which is executed against any execution exceptions.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="executionInterception">The execution interception.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static INoResultCommandProcessor<TFilter> InterceptExceptions<TFilter>(
            this INoResultCommandProcessor<TFilter> source, Action<SusanooExecutionException> executionInterception)
        {
            if (executionInterception == null)
                throw new ArgumentNullException(nameof(executionInterception));

            return
                source.InterceptOrProxyWith(
                    s => new NoResultSetExceptionInterceptionProxy<TFilter>(s, executionInterception));
        }
    }
}
