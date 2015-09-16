using System;
using Susanoo.Processing;
using Susanoo.Transforms;

namespace Susanoo
{
    public static class SingleResultTransformExtensions
    {
        public static ICommandProcessor<TFilter, TResult> ApplyTransforms<TFilter, TResult>(
            this ICommandProcessor<TFilter, TResult> source, params CommandTransform[] transforms)
        {
            if(transforms == null)
                throw new ArgumentNullException(nameof(transforms));

            return source.InterceptOrProxyWith(s => new SingleResultTransformProxy<TFilter, TResult>(s, transforms));
        }
    }
}