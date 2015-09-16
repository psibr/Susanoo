using System;
using Susanoo.Processing;
using Susanoo.Transforms;

namespace Susanoo
{
    /// <summary>
    /// Provides extensions to the pipeline at the single result command processor just prior to execution.
    /// </summary>
    public static class SingleResultSetTransformExtensions
    {
        /// <summary>
        /// Applies a set of transforms to future executions.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">At least one transform is required.</exception>
        public static ICommandProcessor<TFilter, TResult> ApplyTransforms<TFilter, TResult>(
            this ICommandProcessor<TFilter, TResult> source, params CommandTransform[] transforms)
        {
            if (transforms == null)
                throw new ArgumentNullException(nameof(transforms), "At least one transform is required.");

            return source.InterceptOrProxyWith(s => new SingleResultSetTransformProxy<TFilter, TResult>(s, transforms));
        }
    }
}