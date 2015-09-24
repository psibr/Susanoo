using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Processing;

namespace Susanoo.Deserialization
{
    public class Deserializer : IDeserializer
    {
        /// <summary>
        /// Gets the type of the deserialization result element.
        /// </summary>
        /// <value>The type of the deserialization result element.</value>
        public Type DeserializationType { get; }

        private readonly Func<IDataReader, ColumnChecker, IEnumerable> _deserializer;

        public Deserializer(Type deserializationType, Func<IDataReader, ColumnChecker, IEnumerable> deserializer)
        {
            DeserializationType = deserializationType;
            _deserializer = deserializer;
        }

        public IEnumerable Deserialize(IDataReader reader, ColumnChecker columnReport)
        {
            return _deserializer(reader, columnReport);
        }
    }

    public class Deserializer<TResult>
        : IDeserializer<TResult>
    {
        private readonly Func<IDataReader, ColumnChecker, IEnumerable<TResult>> _deserializer;

        public Deserializer(Func<IDataReader, ColumnChecker, IEnumerable<TResult>> deserializer)
        {
            _deserializer = deserializer;
        }

        public Type DeserializationType =>
            typeof(TResult);

        public IEnumerable<TResult> Deserialize(IDataReader reader, ColumnChecker columnReport)
        {
            return _deserializer(reader, columnReport);
        }
    }
}
