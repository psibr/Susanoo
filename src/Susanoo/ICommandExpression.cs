using System;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace Susanoo
{
    /// <summary>
    /// Susanoo's initial step in the command definition Fluent API, in which parameters and command information are provided.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandExpression<TFilter>
        : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        string CommandText { get; }

        /// <summary>
        /// Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        CommandType DBCommandType { get; }

        /// <summary>
        /// Finalizes the pipeline with no result mappings.
        /// </summary>
        ICommandProcessor<TFilter> Finalize();

        /// <summary>
        /// Adds parameters that will always use the same value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterBuilder">The parameter builder.</param>
        /// <returns>ICommandExpression&lt;TResult&gt;.</returns>
        ICommandExpression<TFilter> AddConstantParameter(string parameterName, Func<DbParameter, DbParameter> parameterBuilder);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression);

        /// <summary>
        /// Includes a property of the filter or modifies its inclusion.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression, Action<DbParameter> parameterOptions);

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
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        DbParameter[] BuildParameters(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult> DefineResults<TResult>()
            where TResult : new();

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2> DefineResults<TResult1, TResult2>()
            where TResult1 : new()
            where TResult2 : new();

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> DefineResults<TResult1, TResult2, TResult3>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new();

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> DefineResults<TResult1, TResult2, TResult3, TResult4>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new();

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> DefineResults<TResult1, TResult2, TResult3, TResult4, TResult5>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new();

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <typeparam name="TResult6">The type of the result6.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> DefineResults<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new();

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <typeparam name="TResult6">The type of the result6.</typeparam>
        /// <typeparam name="TResult7">The type of the result7.</typeparam>
        /// <returns>IResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> DefineResults<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
            where TResult7 : new();
    }
}