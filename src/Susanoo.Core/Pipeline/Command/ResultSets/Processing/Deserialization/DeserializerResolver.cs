using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// An extendable or replaceable component that chooses an appropriate way to deserialize an IDataReader to objects.
    /// </summary>
    public class DeserializerResolver : IDeserializerResolver
    {
        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public virtual Func<IDataReader, ColumnChecker, IEnumerable<TResult>>
            Resolve<TResult>(ICommandResultMappingExport mappings)
        {
            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> deserializer;

            if (typeof(TResult) == typeof(DynamicRow) || typeof(TResult) == typeof(object))
                deserializer = DynamicRowDeserializer.Deserialize<TResult>;
            else if (CommandManager.GetDbType(typeof(TResult)) != null)
                deserializer = BuiltInTypeDeserializer.Deserialize<TResult>;
            else
                deserializer = ComplexTypeDeserializer.Compile<TResult>(mappings, typeof(TResult));

            return deserializer;
        }
    }
}