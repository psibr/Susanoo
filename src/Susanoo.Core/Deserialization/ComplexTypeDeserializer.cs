using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Susanoo.Mapping;
using Susanoo.Processing;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Performs deserialization of POCOs
    /// </summary>
    public class ComplexTypeDeserializer
        : IDeserializer
    {
        private readonly IMappingExport _mappings;
        private readonly Func<DbDataReader, ColumnChecker, object> _compiledMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexTypeDeserializer"/> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="type">The type.</param>
        /// <param name="compiledMap">The compiled mapping operations.</param>
        public ComplexTypeDeserializer(IMappingExport mappings, Type type, Func<DbDataReader, ColumnChecker, object> compiledMap)
        {
            _mappings = mappings;
            _compiledMap = compiledMap;
            DeserializationType = type;
        }

        /// <summary>
        /// Gets the type of the deserialization target.
        /// </summary>
        /// <value>The type of the deserialization.</value>
        public Type DeserializationType { get; }

        /// <summary>
        /// Deserializes the reader into a collection of objects.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnReport">The column report.</param>
        /// <returns>IEnumerable.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public IEnumerable Deserialize(DbDataReader reader, ColumnChecker columnReport)
        {
            while (reader.Read())
            {
                yield return _compiledMap(reader, columnReport);
            }
        }
    }

    /// <summary>
    /// Performs deserialization of POCOs
    /// </summary>
    public class ComplexTypeDeserializer<TResult>
        : IDeserializer<TResult>
    {
        private readonly ComplexTypeDeserializer baseDeserializer;
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexTypeDeserializer" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="compiledMap">The compiled mapping operations.</param>
        public ComplexTypeDeserializer(IMappingExport mappings, Func<DbDataReader, ColumnChecker, object> compiledMap)
        {
            baseDeserializer = new ComplexTypeDeserializer(mappings, typeof(TResult), compiledMap);
        }

        /// <summary>
        /// Gets the type of the deserialization target.
        /// </summary>
        /// <value>The type of the deserialization.</value>
        public Type DeserializationType => baseDeserializer.DeserializationType;

        /// <summary>
        /// Deserializes the reader into a collection of objects.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnReport">The column report.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Deserialize(DbDataReader reader, ColumnChecker columnReport)
        {
            return baseDeserializer.Deserialize(reader, columnReport)
                .Cast<TResult>();
        }
    }
}
