#region

using System;
using Susanoo.Mapping;
using Susanoo.Processing;

#endregion

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface ICommandSingleResultExpression<TFilter, TResult> :
        ICommandResultExpression<TFilter>
    {
        /// <summary>
        /// Allows customization of result set.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandSingleResultExpression<TFilter, TResult> ForResults(
            Action<IResultMappingExpression<TFilter, TResult>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        ISingleResultSetCommandProcessor<TFilter, TResult> Realize(string name = null);
    }
}