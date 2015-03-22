using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// Provides deserialization for built in types
    /// </summary>
    public static class BuiltInTypeDeserializer
    {
        /// <summary>
        /// Reads the first value only and casts to built in type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="System.InvalidCastException">Value types cannot cast null.</exception>
        public static IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
        {
            var resultSet = new ListResult<TResult>();
            try
            {
                var fieldCount = reader.FieldCount;
                if (fieldCount > 0)
                {
                    while (reader.Read())
                    {
                        var result = reader.GetValue(0); //Boxing is unavoidable :[

                        result = result == DBNull.Value ? null : result;

                        resultSet.Add((TResult)result);
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                // It is confusing to get a NullReference Exception due to casting,
                // so we rethrow as an invalid cast with more details.
                throw new InvalidCastException("Value types cannot cast null.", ex);
            }
            return resultSet;
        }
    }
}