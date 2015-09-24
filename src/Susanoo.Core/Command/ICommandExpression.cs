#region

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Susanoo.Pipeline;
using Susanoo.Processing;
using Susanoo.ResultSets;

#endregion

namespace Susanoo.Command
{
    /// <summary>
    /// Susanoo's initial step in the CommandBuilder definition Fluent API, in which parameters and CommandBuilder information are
    /// provided.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandExpression<TFilter>
        : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets or sets the CommandBuilder text.
        /// </summary>
        /// <value>The CommandBuilder text.</value>
        string CommandText { get; set; }

            /// <summary>
        /// Realizes the pipeline with no result mappings.
        /// </summary>
        /// <returns>INoResultCommandProcessor&lt;TFilter&gt;.</returns>
        INoResultCommandProcessor<TFilter> Realize();

        /// <summary>
        /// Disables Susanoo's ability to cache a result sets column indexes and names for faster retrieval.
        /// This is typically only needed for stored procedures that return different columns or columns 
        /// in different orders based on criteria in the procedure.
        /// </summary>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        ICommandExpression<TFilter> DoNotStoreColumnIndexes();

        /// <summary>
        /// ADO.NET ignores parameters with NULL values. calling this opts in to send DbNull in place of NULL on standard
        /// parameters.
        /// Properties with modifier Actions do NOT qualify for this behavior
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        ICommandExpression<TFilter> SendNullValues(NullValueMode mode = NullValueMode.FilterOnlyMinimum);

        /// <summary>
        /// Adds parameters that will always use the same value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterBuilder">The parameter builder.</param>
        /// <returns>ICommandExpression&lt;TResult&gt;.</returns>
        ICommandExpression<TFilter> AddConstantParameter(string parameterName, Action<DbParameter> parameterBuilder);

        /// <summary>
        /// Uses the explicit property inclusion mode for including parameters from a potential filter.
        /// </summary>
        /// <returns>ICommandExpression&lt;TResult&gt;.</returns>
        ICommandExpression<TFilter> UseExplicitPropertyInclusionMode();

        /// <summary>
        /// Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter> ExcludeProperty(Expression<Func<TFilter, object>> propertyExpression);

        /// <summary>
        /// Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandExpression<TFilter> ExcludeProperty(string propertyName);

        /// <summary>
        /// Includes a property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression);

        /// <summary>
        /// Includes a property of the filter or modifies its inclusion.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression,
            Action<DbParameter> parameterOptions);

        /// <summary>
        /// Includes a property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        ICommandExpression<TFilter> IncludeProperty(string propertyName);

        /// <summary>
        /// Includes a property of the filter or modifies its inclusion.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        ICommandExpression<TFilter> IncludeProperty(string propertyName, Action<DbParameter> parameterOptions);

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandSingleResultExpression<TFilter, TResult> DefineResults<TResult>();

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <param name="resultTypes">The result types in order.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandMultipleResultExpression<TFilter> DefineResults(params Type[] resultTypes);

    }
}