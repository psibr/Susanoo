using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// An extendable or replaceable component that chooses an appropriate way to deserialize an IDataReader to objects.
    /// </summary>
    public interface IDeserializerResolver
    {
        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        Func<IDataReader, ColumnChecker, IEnumerable<TResult>>
            Resolve<TResult>(ICommandResultMappingExport mappings)
            where TResult : new();
    }
}