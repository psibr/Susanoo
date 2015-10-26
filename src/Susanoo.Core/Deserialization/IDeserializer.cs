using Susanoo.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Interface IDeserializer
    /// </summary>
    public interface IDeserializer
    {
        /// <summary>
        /// Gets the type of the deserialization target.
        /// </summary>
        /// <value>The type of the deserialization.</value>
        Type DeserializationType { get; }

        /// <summary>
        /// Deserializes the reader into a collection of objects.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnReport">The column report.</param>
        /// <returns>IEnumerable.</returns>
        IEnumerable Deserialize(IDataReader reader, ColumnChecker columnReport);
    }

    /// <summary>
    /// Interface IDeserializer
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public interface IDeserializer<out TResult>
    {
        /// <summary>
        /// Gets the type of the deserialization target.
        /// </summary>
        /// <value>The type of the deserialization.</value>
        Type DeserializationType { get; }

        /// <summary>
        /// Deserializes the reader into a collection of objects.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnReport">The column report.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Deserialize(IDataReader reader, ColumnChecker columnReport);
    }
}