using System;
using Susanoo.Mapping;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Provides the ability to map objects with constructors or special considerations.
    /// </summary>
    public interface IDeserializerFactory
    {
        /// <summary>
        /// Determines whether this deserializer applies to the type.
        /// </summary>
        /// <returns><c>true</c> if this instance can deserialize; otherwise, <c>false</c>.</returns>
        bool CanDeserialize(Type type);

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IDeserializer<TResult> BuildDeserializer<TResult>(IMappingExport mappings);

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IDeserializer BuildDeserializer(Type resultType, IMappingExport mappings);
    }
}