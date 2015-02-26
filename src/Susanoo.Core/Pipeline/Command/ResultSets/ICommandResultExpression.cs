#region

using Susanoo.Pipeline.Command.ResultSets.Mapping;
using Susanoo.Pipeline.Command.ResultSets.Processing;
using System;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult> :
        ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of result set.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultExpression<TFilter, TResult> ForResults(
            Action<IResultMappingExpression<TFilter, TResult>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        ICommandProcessor<TFilter, TResult> Realize(string name = null);

        /// <summary>
        /// Builds the where filter.
        /// </summary>
        /// <param name="optionsObject">The options object.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        ICommandResultExpression<TFilter, TResult> BuildWhereFilter(object optionsObject);
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult1, TResult2> 
        : ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of a type of result set.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2> Realize(string name = null);
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> 
        : ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of a type of result set.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3> Realize(string name = null);
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> :
        ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of a type of result set.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> Realize(string name = null);
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> :
        ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of a type of result set.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultsOfType
            <TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> Realize(string name = null);
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> :
        ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of a type of result set.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> ForResultsOfType
            <TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Realize(
            string name = null);
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    /// <typeparam name="TResult7">The type of the 7th result.</typeparam>
    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6,
        TResult7> : ICommandResultExpressionCore<TFilter>
    {
        /// <summary>
        /// Allows customization of a type of result set.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings);

        /// <summary>
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Realize(
            string name = null);
    }
}