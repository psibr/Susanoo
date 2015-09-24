using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// An extendable or replaceable component that chooses an appropriate way to deserialize an IDataReader to objects.
    /// </summary>
    public interface IDeserializerResolver
    {
        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        IDeserializer<TResult> ResolveDeserializer<TResult>(ICommandResultMappingExporter mappings);

        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        IDeserializer ResolveDeserializer(Type resultType, ICommandResultMappingExporter mappings);
    }
}