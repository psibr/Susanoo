using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public interface ICommandResultExpression<TFilter>
    {
        ICommandExpression<TFilter> CommandExpression { get; }

        IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export<TResultType>()
            where TResultType : new();
    }

    public interface ICommandResultExpression<TFilter, TResult> : ICommandResultExpression<TFilter>
        where TResult : new ()
    {
        ICommandResultExpression<TFilter, TResult> ForResultSet(
            Action<IResultMappingExpression<TFilter, TResult>> mappings);

        ICommandProcessor<TFilter, TResult> Finalize();


    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2> : ICommandResultExpression<TFilter>
        where TResult1 : new()
        where TResult2 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> : ICommandResultExpression<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> : ICommandResultExpression<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> : ICommandResultExpression<TFilter>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new();
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> : ICommandResultExpression<TFilter>
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
    }

    public interface ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> : ICommandResultExpression<TFilter>
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
    }
}
