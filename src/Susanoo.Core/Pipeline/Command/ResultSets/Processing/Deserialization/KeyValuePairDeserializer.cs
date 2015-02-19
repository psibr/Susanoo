using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.XPath;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// Maps properties to a KeyValuePair using Activator
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public class KeyValuePairDeserializer<TResult> : ICustomDeserializer<TResult>
    {
        private readonly IDictionary<string, IPropertyMapping> _props;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairDeserializer{TResult}"/> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        public KeyValuePairDeserializer(ICommandResultMappingExport mappings)
        {
            _props = mappings.Export(typeof(TResult));
        }

        /// <summary>
        /// Deserializes into a KeyValuePair from a data reader.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <param name="checker">The column object.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Deserialize(IDataReader reader, ColumnChecker checker)
        {
            var keyAlias = "Key";
            IPropertyMapping keyMapping;
            var valueAlias = "Value";
            IPropertyMapping valueMapping;

            if (_props.TryGetValue("Key", out keyMapping))
                keyAlias = keyMapping.ActiveAlias;
            if (_props.TryGetValue("Value", out valueMapping))
                valueAlias = valueMapping.ActiveAlias;

            var resultType = typeof(TResult);

            var genericTypeArguments = resultType.GetGenericArguments();

            IList resultSet = new List<TResult>();

            checker = checker ?? new ColumnChecker();

            while (reader.Read())
            {
                resultSet.Add(Activator.CreateInstance(typeof(TResult),
                    Convert.ChangeType(reader.GetValue(checker.HasColumn(reader, keyAlias)), genericTypeArguments[0]),
                    Convert.ChangeType(reader.GetValue(checker.HasColumn(reader, valueAlias)), genericTypeArguments[1])));

            }

            return (IEnumerable<TResult>)resultSet;
        }
    }
}
