using System;
using Susanoo.Deserialization;
using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// Builds a command processor that returns multiple result sets.
    /// </summary>
    public class MultipleResultSetCommandProcessorFactory : IMultipleResultSetCommandProcessorFactory
    {
        private readonly IDeserializerResolver _deserializerResolver;

        /// <summary>
        /// Registers dependencies for the command processor.
        /// </summary>
        /// <param name="deserializerResolver">The deserializer resolver.</param>
        public MultipleResultSetCommandProcessorFactory(IDeserializerResolver deserializerResolver)
        {
            _deserializerResolver = deserializerResolver;
        }

        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        /// <param name="resultTypes">The result types.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public virtual IMultipleResultSetCommandProcessor<TFilter> BuildCommandProcessor<TFilter>(ICommandResultInfo<TFilter> mappings,
            string name = null, params Type[] resultTypes) => 
                new MultipleResultSetCommandProcessor<TFilter>(_deserializerResolver, mappings, name, resultTypes);
    }
}
