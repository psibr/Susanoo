using System.Collections.Generic;
using System.Data;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// Provides the ability to map objects with constructors or special considerations.
    /// </summary>
    public interface ICustomDeserializer<out TResult>
    {
        /// <summary>
        /// Deserializes into a complex object from a data reader.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="checker">The column object.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Deserialize(IDataReader reader, ColumnChecker checker);
    }
}