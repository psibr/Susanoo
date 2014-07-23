using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo
{
    public interface ICommandResultExpressionCore<TFilter>
        : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        ICommandExpression<TFilter> CommandExpression { get; }

        /// <summary>
        /// Exports all Property mapping configurations for a result type.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;.</returns>
        IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export<TResultType>()
            where TResultType : new();

        /// <summary>
        /// Converts the command result expression to a single result expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult> ToSingleResult<TResult>()
            where TResult : new();
    }

    public interface ICommandResultExpression<TFilter, TResult> : ICommandResultExpressionCore<TFilter>
        where TResult : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult> ForResultSet(
            Action<IResultMappingExpression<TFilter, TResult>> mappings);

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        ICommandProcessor<TFilter, TResult> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        /// <summary>
        /// Moves fluent API into mapping options for a result type, most commonly aliasing properties.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        /// <summary>
        /// Prepares the command for execution.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Finalize();
    }
}