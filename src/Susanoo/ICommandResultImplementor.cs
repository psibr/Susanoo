using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo
{
    public interface ICommandResultImplementor<TFilter> : IFluentPipelineFragment
    {
        IResultMappingExpression<TFilter, TResult> RetrieveMapping<TResult>()
            where TResult : new();

        void StoreMapping<TResult>(Action<IResultMappingExpression<TFilter, TResult>> mapping)
            where TResult : new();

        IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export<TResultType>()
            where TResultType : new();
    }
}