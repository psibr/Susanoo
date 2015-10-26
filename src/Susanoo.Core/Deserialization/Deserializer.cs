using Susanoo.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Represents a deserialization method and target type
    /// </summary>
    public class Deserializer : IDeserializer
    {
        /// <summary>
        /// Gets the type of the deserialization result element.
        /// </summary>
        /// <value>The type of the deserialization result element.</value>
        public Type DeserializationType { get; }

        private readonly Func<IDataReader, ColumnChecker, IEnumerable> _deserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deserializer"/> class.
        /// </summary>
        /// <param name="deserializationType">Type of the deserialization.</param>
        /// <param name="deserializer">The deserializer.</param>
        public Deserializer(Type deserializationType, Func<IDataReader, ColumnChecker, IEnumerable> deserializer)
        {
            DeserializationType = deserializationType;
            _deserializer = deserializer;
        }

        /// <summary>
        /// Deserializes the reader into a collection of objects.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnReport">The column report.</param>
        /// <returns>IEnumerable.</returns>
        public IEnumerable Deserialize(IDataReader reader, ColumnChecker columnReport)
        {
            return _deserializer(reader, columnReport);
        }
    }

    /// <summary>
    /// Represents a deserialization method and target type
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class Deserializer<TResult>
            : IDeserializer<TResult>
    {
        private readonly Func<IDataReader, ColumnChecker, IEnumerable<TResult>> _deserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deserializer{TResult}"/> class.
        /// </summary>
        /// <param name="deserializer">The deserializer.</param>
        public Deserializer(Func<IDataReader, ColumnChecker, IEnumerable<TResult>> deserializer)
        {
            _deserializer = deserializer;
        }

        /// <summary>
        /// Gets or sets the type of the deserialization target.
        /// </summary>
        /// <value>The type of the deserialization.</value>
        public Type DeserializationType { get; } = typeof(TResult);

        /// <summary>
        /// Deserializes the reader into a collection of objects.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnReport">The column report.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Deserialize(IDataReader reader, ColumnChecker columnReport)
        {
            return _deserializer(reader, columnReport);
        }
    }
}
