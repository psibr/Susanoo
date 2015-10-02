using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Susanoo.Command;
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
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        public override IResultSetReader Execute(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            var results = new IEnumerable[_mappers.Length];

            try
            {
                using (var record = databaseManager
                    .ExecuteDataReader(
                        executableCommandInfo.CommandText,
                        executableCommandInfo.DbCommandType,
                        Timeout,
                        executableCommandInfo.Parameters))
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
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            IResultSetReader finalResults = new ResultSetReader(results);

            return finalResults;
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        public override async Task<IResultSetReader> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            var results = new IEnumerable[_mappers.Length];

            try
            {
                using (var record = await databaseManager
                    .ExecuteDataReaderAsync(
                        executableCommandInfo.CommandText,
                        executableCommandInfo.DbCommandType,
                        Timeout,
                        cancellationToken,
                        executableCommandInfo.Parameters))
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
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            IResultSetReader finalResults = new ResultSetReader(results);

            return finalResults;
        }
    }
}