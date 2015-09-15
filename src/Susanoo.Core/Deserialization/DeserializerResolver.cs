using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// An extendable or replaceable component that chooses an appropriate way to deserialize an IDataReader to objects.
    /// </summary>
    public class DeserializerResolver : IDeserializerResolver
    {
        private readonly IEnumerable<IDeserializerFactory> _deserializerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializerResolver"/> class.
        /// </summary>
        /// <param name="deserializerFactories">The deserializer factories.</param>
        public DeserializerResolver(IEnumerable<IDeserializerFactory> deserializerFactories)
        {
            var workflow = new List<IDeserializerFactory>
            {
                new DynamicRowDeserializerFactory(),
                new BuiltInTypeDeserializerFactory(),
                //parameter factories go here in the flow
                new ComplexTypeDeserializerFactory()
            };

            workflow.InsertRange(2, deserializerFactories);

            _deserializerFactories = workflow;
        }

        /// <summary>
        /// Retrieves and compiles, if necessary, an appropriate type deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public virtual Func<IDataReader, ColumnChecker, IEnumerable<TResult>>
            ResolveDeserializer<TResult>(ICommandResultMappingExport mappings)
        {
            var type = typeof (TResult);

            var factory = _deserializerFactories.First(df => df.CanDeserialize(type));

            return factory.BuildDeserializer<TResult>(mappings);
        }
    }
}