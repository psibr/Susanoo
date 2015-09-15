using System;
using System.Collections.Generic;
using System.Data;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// An extendable or replaceable component that chooses an appropriate way to deserialize an IDataReader to objects.
    /// </summary>
    public class DeserializerResolver : IDeserializerResolver
    {
        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public virtual Func<IDataReader, ColumnChecker, IEnumerable<TResult>>
            ResolveDeserializer<TResult>(ICommandResultMappingExport mappings)
        {
            var type = typeof (TResult);

            IDeserializerFactory dynamicRowDeserializer = new DynamicRowDeserializerFactory(); //singleton
            IDeserializerFactory builtInDeserializer = new BuiltInTypeDeserializerFactory(); //singleton
            IDeserializerFactory complexTypeDeserializer = new ComplexTypeDeserializerFactory(); //singleton

            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> deserializer;

            if (dynamicRowDeserializer.CanDeserialize(type))
                deserializer = dynamicRowDeserializer.BuildDeserializer<TResult>(mappings);
            else if (builtInDeserializer.CanDeserialize(type))
                deserializer = builtInDeserializer.BuildDeserializer<TResult>(mappings);
            else
            {
                deserializer = ResolveCustomDeserializer<TResult>(mappings) //Custom deserializers
                    ?? complexTypeDeserializer.BuildDeserializer<TResult>(mappings);
            }

            return deserializer;
        }

        /// <summary>
        /// Resolves any custom deserializers.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public virtual Func<IDataReader, ColumnChecker, IEnumerable<TResult>> ResolveCustomDeserializer<TResult>(
            ICommandResultMappingExport mappings)
        {
            var type = typeof(TResult);

            var kvpDeserializer = new KeyValuePairDeserializerFactory();
            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> customDeserializer = null;

            if (kvpDeserializer.CanDeserialize(type))
            {
                customDeserializer = kvpDeserializer.BuildDeserializer<TResult>(mappings);
            }

            return customDeserializer;
        }
    }
}