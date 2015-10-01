using System;
using System.Collections.Generic;
using System.Linq;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// An extendable or replaceable component that chooses an appropriate way to deserialize an IDataReader to objects.
    /// </summary>
    public class DeserializerResolver 
        : IDeserializerResolver
    {
        private readonly IEnumerable<IDeserializerFactory> _deserializerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializerResolver"/> class.
        /// </summary>
        /// <param name="deserializerFactories">The deserializer factories.</param>
        /// <exception cref="ArgumentNullException"><paramref name="deserializerFactories" /> is null.</exception>
        public DeserializerResolver(params IDeserializerFactory[] deserializerFactories)
        {
            var workflow = new List<IDeserializerFactory>
            {
                new DynamicRowDeserializerFactory(),
                new BuiltInTypeDeserializerFactory(),
                //parameter factories go here in the flow
                new ComplexTypeDeserializerFactory()
            };

            try
            {
                workflow.InsertRange(2, deserializerFactories);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            _deserializerFactories = workflow;
        }

        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public virtual IDeserializer<TResult>
            ResolveDeserializer<TResult>(ICommandResultMappingExporter mappings)
        {

            var factory = _deserializerFactories.First(df => df.CanDeserialize(typeof (TResult)));

            return factory.BuildDeserializer<TResult>(mappings);
        }

        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public IDeserializer ResolveDeserializer(Type resultType, ICommandResultMappingExporter mappings)
        {
            var factory = _deserializerFactories.First(df => df.CanDeserialize(resultType));

            return factory.BuildDeserializer(resultType, mappings);
        }
    }
}