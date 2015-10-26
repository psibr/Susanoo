using Susanoo.Deserialization;
using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// Builds a command processor that returns a single result set.
    /// </summary>
    public class SingleResultSetCommandProcessorFactory : ISingleResultSetCommandProcessorFactory
    {
        private readonly IDeserializerResolver _deserializerResolver;

        /// <summary>
        /// Provides dependencies for the processors.
        /// </summary>
        /// <param name="deserializerResolver">The deserializer resolver.</param>
        public SingleResultSetCommandProcessorFactory(IDeserializerResolver deserializerResolver)
        {
            _deserializerResolver = deserializerResolver;
        }

        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public virtual ISingleResultSetCommandProcessor<TFilter, TResult> BuildCommandProcessor<TFilter, TResult>(
            ICommandResultInfo<TFilter> mappings) =>
                new SingleResultSetCommandProcessor<TFilter, TResult>(_deserializerResolver, mappings);
    }
}
