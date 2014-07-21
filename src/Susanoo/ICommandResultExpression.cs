using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo
{
    public interface ICommandResultExpressionCore<TFilter>
        : IFluentPipelineFragment
    {
        ICommandExpression<TFilter> CommandExpression { get; }

        IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export<TResultType>()
            where TResultType : new();

        ICommandResultExpression<TFilter, TResult> ToSingleResult<TResult>()
            where TResult : new();
    }

    public interface ICommandResultExpression<TFilter, TResult> : ICommandResultExpressionCore<TFilter>
        where TResult : new()
    {
        ICommandResultExpression<TFilter, TResult> ForResultSet(
            Action<IResultMappingExpression<TFilter, TResult>> mappings);

        ICommandProcessor<TFilter, TResult> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        ICommandProcessor<TFilter, TResult1, TResult2> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        ICommandProcessor<TFilter, TResult1, TResult2, TResult3> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> Finalize();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> : ICommandResultExpressionCore<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();

        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Finalize();
    }
}