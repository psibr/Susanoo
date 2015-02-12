using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// Provides deserialization for dynamic and a way to get a Key-Value-Pair.
    /// </summary>
    public static class BuiltInTypeDeserializer
    {
        /// <summary>
        /// Dumps all columns into an array for simple use cases.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>dynamic.</returns>
        public static IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
        {
            var resultSet = new List<object>();

            var fieldCount = reader.FieldCount;
            if (fieldCount > 0)
            {
                while (reader.Read())
                {

                    resultSet.Add(reader.GetValue(0));
                }
            }

            return resultSet.Cast<TResult>();
        }
    }
}