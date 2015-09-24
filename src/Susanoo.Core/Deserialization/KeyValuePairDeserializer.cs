using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Susanoo.Mapping.Properties;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Performs deserialization of KeyValuePairs
    /// </summary>
    public class KeyValuePairDeserializer
    {
        private readonly Type _type;
        private readonly IDictionary<string, IPropertyMapping> _props;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairDeserializer" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="type">The type.</param>
        public KeyValuePairDeserializer(ICommandResultMappingExporter mappings, Type type)
        {
            _type = type;
            _props = mappings.Export(type);
        }

        /// <summary>
        /// Deserializes into a KeyValuePair from a data reader.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="checker">The column object.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
        {
            return Deserialize(reader, checker).Cast<TResult>();
        }

        /// <summary>
        /// Deserializes into a KeyValuePair from a data reader.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <param name="checker">The column object.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable Deserialize(IDataReader reader, ColumnChecker checker)
        {
            var keyAlias = "Key";
            IPropertyMapping keyMapping;
            var valueAlias = "Value";
            IPropertyMapping valueMapping;

            if (_props.TryGetValue("Key", out keyMapping))
                keyAlias = keyMapping.ActiveAlias;
            if (_props.TryGetValue("Value", out valueMapping))
                valueAlias = valueMapping.ActiveAlias;

            var resultType = _type;

            var genericTypeArguments = resultType.GetGenericArguments();

            IList resultSet = new ArrayList();

            checker = checker ?? new ColumnChecker();

            while (reader.Read())
            {
                resultSet.Add(Activator.CreateInstance(resultType,
                    Convert.ChangeType(reader.GetValue(checker.HasColumn(reader, keyAlias)), genericTypeArguments[0]),
                    Convert.ChangeType(reader.GetValue(checker.HasColumn(reader, valueAlias)), genericTypeArguments[1])));

            }

            return resultSet;
        }
    }
}