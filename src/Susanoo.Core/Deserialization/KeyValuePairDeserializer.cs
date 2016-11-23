using Susanoo.Mapping;
using Susanoo.Mapping.Properties;
using Susanoo.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Performs deserialization of KeyValuePairs
    /// </summary>
    public class KeyValuePairDeserializer
        : IDeserializer
    {
        private readonly IDictionary<string, IPropertyMapping> _props;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairDeserializer" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="type">The type.</param>
        public KeyValuePairDeserializer(IMappingExport mappings, Type type)
        {
            DeserializationType = type;
            _props = mappings.Export();
        }

        /// <summary>
        /// Deserializes into a KeyValuePair from a data reader.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="checker">The column object.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Deserialize<TResult>(DbDataReader reader, ColumnChecker checker)
        {
            return Deserialize(reader, checker).Cast<TResult>();
        }

        /// <summary>
        /// Gets the type of the deserialization target.
        /// </summary>
        /// <value>The type of the deserialization.</value>
        public Type DeserializationType { get; }

        /// <summary>
        /// Deserializes into a KeyValuePair from a data reader.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <param name="checker">The column object.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable Deserialize(DbDataReader reader, ColumnChecker checker)
        {
            var keyAlias = "Key";
            IPropertyMapping keyMapping;
            var valueAlias = "Value";
            IPropertyMapping valueMapping;

            if (_props.TryGetValue("Key", out keyMapping))
                keyAlias = keyMapping.ActiveAlias;
            if (_props.TryGetValue("Value", out valueMapping))
                valueAlias = valueMapping.ActiveAlias;

            var resultType = DeserializationType;

            var genericTypeArguments = resultType.GetTypeInfo().GetGenericArguments();

            checker = checker ?? new ColumnChecker(reader.FieldCount);

            while (reader.Read())
            {
                yield return Activator.CreateInstance(resultType,
                    Convert.ChangeType(reader.GetValue(checker.HasColumn(reader, keyAlias)), genericTypeArguments[0]),
                    Convert.ChangeType(reader.GetValue(checker.HasColumn(reader, valueAlias)), genericTypeArguments[1]));

            }
        }
    }

    /// <summary>
    /// Performs deserialization of KeyValuePairs
    /// </summary>
    public class KeyValuePairDeserializer<TResult>
        : IDeserializer<TResult>
    {
        private readonly KeyValuePairDeserializer baseDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairDeserializer" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        public KeyValuePairDeserializer(IMappingExport mappings) 
        {
            baseDeserializer = new KeyValuePairDeserializer(mappings, typeof(TResult));
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