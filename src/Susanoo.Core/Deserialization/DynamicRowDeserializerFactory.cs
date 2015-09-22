using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Provides deserialization for dynamic and a way to geta Key-Value-Pair.
    /// </summary>
    public class DynamicRowDeserializerFactory
        : IDeserializerFactory
    {
        /// <summary>
        /// Determines whether this deserializer applies to the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this instance can deserialize; otherwise, <c>false</c>.</returns>
        public bool CanDeserialize(Type type)
        {
            return type == typeof(DynamicRow) || type == typeof(object);
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public Func<IDataReader, ColumnChecker, IEnumerable<TResult>> BuildDeserializer<TResult>(ICommandResultMappingExport mappings)
        {
            return Deserialize<TResult>;
        }

        /// <summary>
        /// Dumps all columns into an array for simple use cases.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>dynamic.</returns>
        public IEnumerable<TResult> Deserialize<TResult>(IDataReader reader, ColumnChecker checker)
        {
            IList resultSet = new ListResult<TResult>();

            checker = checker ?? new ColumnChecker();

            var fieldCount = reader.FieldCount;

            var needsFieldNames = fieldCount > checker.Count;

            while (reader.Read())
            {
                object[] values;
                if (needsFieldNames)
                {
                    var obj = new List<object>();
                    for (var i = 0; i < fieldCount; i++)
                    {
                        checker.HasColumn(reader, i);
                        obj.Add(reader.GetValue(i));
                    }

                    values = obj.ToArray();
                }
                else
                {
                    values = new object[fieldCount];
                    reader.GetValues(values);
                }

                resultSet.Add(new DynamicRow(checker, values));
            }

            ((ListResult<TResult>)resultSet).BuildReport(checker);

            return (IEnumerable<TResult>)resultSet;
        }
    }
}