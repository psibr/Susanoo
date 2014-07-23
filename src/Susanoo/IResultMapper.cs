using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo
{
    /// <summary>
    /// Represents an object that can map a resultset.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMapper<TResult> : IFluentPipelineFragment
    {
        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> MapResult(IDataReader record, Func<IDataRecord, object> mapping);

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> MapResult(IDataReader record);
    }
}