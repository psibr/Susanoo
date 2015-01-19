#region

using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Susanoo.Pipeline.Command.ResultSets;
using Susanoo.Pipeline.Command.ResultSets.Processing;

#endregion

namespace Susanoo.Pipeline.Command
{
    /// <summary>
    /// Susanoo's initial step in the command definition Fluent API, in which parameters and command information are
    /// provided.
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
        CommandType DbCommandType { get; }

        /// <summary>
        /// Realizes the pipeline with no result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter&gt;.</returns>
        ICommandProcessor<TFilter> Realize();

        /// <summary>
        /// Gets a value indicating whether storing column information is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow store column information]; otherwise, <c>false</c>.</value>
        bool AllowStoringColumnInfo { get; }

        /// <summary>
        /// Disables Susanoo's ability to cache a result sets column indexes and names for faster retrieval.
        /// This is typically only needed for stored procedures that return different columns or columns in different orders based on criteria in the procedure.
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
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        DbParameter[] BuildParameters(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters);

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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> DefineResults
            <TResult1, TResult2, TResult3, TResult4>()
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> DefineResults
            <TResult1, TResult2, TResult3, TResult4, TResult5>()
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> DefineResults
            <TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>()
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            DefineResults<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
            where TResult7 : new();
    }

    /// <summary>
    /// Opt-in levels for sending null values in commands.
    /// </summary>
    public enum NullValueMode
    {
        /// <summary>
        /// Default option, standard ADO.NET behavior, values of null exclude the parameter from the parameter set.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Replaces null with DbNull on filter properties when no modifier action is provided.
        /// </summary>
        FilterOnlyMinimum,

        /// <summary>
        /// Replaces null with DbNull on all filter properties.
        /// </summary>
        FilterOnlyFull,

        /// <summary>
        /// Replaces null with DbNull on explicit parameters only.
        /// </summary>
        ExplicitParametersOnly,

        /// <summary>
        /// Replaces null with DbNull on all parameters EXCEPT constants.
        /// </summary>
        Full
    }
}