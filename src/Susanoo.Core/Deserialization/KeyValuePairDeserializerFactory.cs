using System;
using System.Collections.Generic;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Maps properties to a KeyValuePair using Activator
    /// </summary>
    public class KeyValuePairDeserializerFactory
        : IDeserializerFactory
    {
        /// <summary>
        /// Determines whether this deserializer applies to the type.
        /// </summary>
        /// <returns><c>true</c> if this instance can deserialize; otherwise, <c>false</c>.</returns>
        public bool CanDeserialize(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof (KeyValuePair<,>);
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IDeserializer<TResult> BuildDeserializer<TResult>(ICommandResultMappingExporter mappings)
        {
            return new Deserializer<TResult>(new KeyValuePairDeserializer(mappings, typeof (TResult)).Deserialize<TResult>);
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IDeserializer BuildDeserializer(Type resultType, ICommandResultMappingExporter mappings)
        {
            return new Deserializer(resultType, new KeyValuePairDeserializer(mappings, resultType).Deserialize);
        }
    }
}
