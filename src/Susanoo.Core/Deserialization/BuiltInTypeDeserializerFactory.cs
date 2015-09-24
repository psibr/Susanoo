using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public IDeserializer<TResult> BuildDeserializer<TResult>(ICommandResultMappingExporter mappings)
        {
            return new Deserializer<TResult>(Deserialize<TResult>);
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IDeserializer BuildDeserializer(Type resultType, ICommandResultMappingExporter mappings)
        {
            return new Deserializer(resultType, Deserialize);
        }

        /// <summary>
        /// Reads the first value only and casts to built in type.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable Deserialize(IDataReader reader, ColumnChecker checker)
        {
            var resultSet = new ArrayList();
            var fieldCount = reader.FieldCount;
            if (fieldCount > 0)
            {
                while (reader.Read())
                {
                    var result = reader.GetValue(0); //Boxing is unavoidable :[

                    result = result == DBNull.Value ? null : result;

                    resultSet.Add(result);
                }
            }

            return resultSet;
        }

        /// <summary>
        /// Deserializes the specified reader.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The checker.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="System.InvalidCastException">Value types cannot cast null.</exception>
        public IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
        {

            try
            {
                return Deserialize(reader, checker).Cast<TResult>();
            }
            catch (NullReferenceException ex)
            {
                // It is confusing to get a NullReference Exception due to casting,
                // so we rethrow as an invalid cast with more details.
                throw new InvalidCastException("Value types cannot cast null.", ex);
            }
        }
    }
}