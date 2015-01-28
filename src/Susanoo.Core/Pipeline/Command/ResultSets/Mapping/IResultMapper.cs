#region

using System;
using System.Collections.Generic;
using System.Data;
using Susanoo.Pipeline.Command.ResultSets.Processing;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets.Mapping
{
    /// <summary>
    /// Performs the actual map operation between an IDataRecord and a result type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMapper<TResult> : IResultMapper, IFluentPipelineFragment
    {
        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="checker">The column checker.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> MapResult(IDataReader record, ColumnChecker checker,
            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> mapping);

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        new IEnumerable<TResult> MapResult(IDataReader record);
    }

    /// <summary>
    /// Performs the actual map operation between an IDataRecord and a result type.
    /// </summary>
    public interface IResultMapper : IFluentPipelineFragment
    {

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        object MapResult(IDataReader record);
    }
}