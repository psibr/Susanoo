#region

using System;
using System.Collections.Generic;
using System.Data;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Performs the actual map operation between an IDataRecord and a result type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMapper<TResult> : IFluentPipelineFragment
    {
        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="checker">The column checker.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> MapResult(IDataReader record, ColumnChecker checker, Func<IDataReader, ColumnChecker, IEnumerable<TResult>> mapping);

        /// <summary>
        ///     Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> MapResult(IDataReader record);
    }
}