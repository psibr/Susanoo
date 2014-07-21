using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo
{
    public interface IResultMapper<TResult> : IFluentPipelineFragment
    {
        IEnumerable<TResult> MapResult(IDataReader record, Func<IDataRecord, object> mapping);

        IEnumerable<TResult> MapResult(IDataReader record);
    }
}