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
        /// Reads the first value only.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>dynamic.</returns>
        public static IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
        {
            var resultSet = new ListResult<TResult>();

            var fieldCount = reader.FieldCount;
            if (fieldCount > 0)
            {
                while (reader.Read())
                {

                    resultSet.Add((TResult)reader.GetValue(0));
                }
            }

            return resultSet;
        }
    }
}