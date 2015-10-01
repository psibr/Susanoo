using System;
using System.Collections;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using Susanoo.Deserialization;
using Susanoo.Exceptions;
using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public sealed class MultipleResultSetCommandProcessor<TFilter> 
        : MultipleResultSetCommandProcessorStructure<TFilter>,
            IMultipleResultSetCommandProcessor<TFilter>
    {
        private readonly IDeserializerResolver _deserializerResolver;

        private readonly IDeserializer[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter}" />
        /// class.
        /// </summary>
        /// <param name="deserializerResolver">The deserializer resolver.</param>
        /// <param name="commandResultInfo">The CommandBuilder result information.</param>
        /// <param name="name">The name.</param>
        /// <param name="resultTypes">The result types.</param>
        public MultipleResultSetCommandProcessor(IDeserializerResolver deserializerResolver,
            ICommandResultInfo<TFilter> commandResultInfo, string name, Type[] resultTypes)
        {
            CommandResultInfo = commandResultInfo;

            _deserializerResolver = deserializerResolver;

            _mappers = resultTypes
                .Select(t => _deserializerResolver.ResolveDeserializer(t, commandResultInfo.GetExporter()))
                .ToArray();

            CommandManager.Instance.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash =>
            CommandResultInfo.CacheHash
                + HashBuilder.Compute(_mappers.Aggregate(string.Empty, (s, deserializer)
                    => s + deserializer.DeserializationType.AssemblyQualifiedName));

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override IMultipleResultSetCommandProcessor<TFilter> InterceptOrProxyWith(Func<IMultipleResultSetCommandProcessor<TFilter>, IMultipleResultSetCommandProcessor<TFilter>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        public override IResultSetReader Execute(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new IEnumerable[_mappers.Length];

            var parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject,
                explicitParameters);

            try
            {
                using (var record = databaseManager
                    .ExecuteDataReader(
                        CommandBuilderInfo.CommandText,
                        CommandBuilderInfo.DbCommandType,
                        Timeout,
                        parameters))
                {
                    var ix = 0;

                    do
                    {
                        results[ix] = _mappers[ix].Deserialize(record, RetrieveColumnIndexInfo());
                        ix++;
                    } while (ix < results.Length && record.NextResult());
                }
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, CommandBuilderInfo, Timeout, parameters);
            }

            IResultSetReader finalResults = new ResultSetReader(results);

            return finalResults;
        }
    }
}