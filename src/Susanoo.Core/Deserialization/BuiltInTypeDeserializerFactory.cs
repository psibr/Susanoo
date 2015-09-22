using System;
using System.Collections.Generic;
using System.Data;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Provides deserialization for built in types
    /// </summary>
    public class BuiltInTypeDeserializerFactory 
        : IDeserializerFactory
    {
        /// <summary>
        /// Determines whether this deserializer applies to the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this instance can deserialize; otherwise, <c>false</c>.</returns>
        public bool CanDeserialize(Type type)
        {
            return CommandManager.GetDbType(type) != null;
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Func<IDataReader, ColumnChecker, IEnumerable<TResult>> BuildDeserializer<TResult>(ICommandResultMappingExport mappings)
        {
            return Deserialize<TResult>;
        }

        /// <summary>
        /// Reads the first value only and casts to built in type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="System.InvalidCastException">Value types cannot cast null.</exception>
        public IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
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