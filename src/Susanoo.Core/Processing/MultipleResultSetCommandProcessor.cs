using Susanoo.Command;
using Susanoo.Deserialization;
using Susanoo.Exceptions;
using Susanoo.ResultSets;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="resultTypes">The result types.</param>
        public MultipleResultSetCommandProcessor(IDeserializerResolver deserializerResolver,
            ICommandResultInfo<TFilter> commandResultInfo, Type[] resultTypes)
        {
            CommandResultInfo = commandResultInfo;

            _deserializerResolver = deserializerResolver;

            _mappers = resultTypes
                .Select(t => _deserializerResolver.ResolveDeserializer(t, commandResultInfo))
                .ToArray();
        }

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
        public override IResultSetCollection Execute(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            var results = new IEnumerable[_mappers.Length];
            IResultSetCollection finalResults;
            try
            {
                var record = databaseManager
                    .ExecuteDataReader(
                        executableCommandInfo.CommandText,
                        executableCommandInfo.DbCommandType,
                        Timeout,
                        executableCommandInfo.Parameters);

                var ix = 0;

                do
                {
                    results[ix] = _mappers[ix].Deserialize(record, RetrieveColumnIndexInfo());
                    ix++;

                } while (ix < results.Length);

                if (StreamingScope.Current == null)
                {
                    try
                    {
                        for (int i = 0; i < results.Length; i++)
                        {
                            results[i] = results[i].Cast<object>().ToList();

                            if (!record.NextResult())
                            {
                                for (int j = i + 1; j < results.Length; j++)
                                {
                                    results[j] = null;
                                }

                                break;
                            }
                        }

                        finalResults = new ResultSetCollection(results, null);
                    }
                    finally
                    {
                        record.Dispose();
                    }
                }
                else
                {
                    StreamingScope.Current.Enlist(record);
                    finalResults = new ResultSetCollection(results, record);
                }
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

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
        public override async Task<IResultSetCollection> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            var results = new IEnumerable[_mappers.Length];
            IResultSetCollection finalResults;

            try
            {
                var record = await databaseManager
                    .ExecuteDataReaderAsync(
                        executableCommandInfo.CommandText,
                        executableCommandInfo.DbCommandType,
                        Timeout,
                        cancellationToken,
                        executableCommandInfo.Parameters)
                    .ConfigureAwait(false);

                var ix = 0;

                do
                {
                    results[ix] = _mappers[ix].Deserialize(record, RetrieveColumnIndexInfo());
                    ix++;

                } while (ix < results.Length);

                if (StreamingScope.Current == null)
                {
                    try
                    {
                        for (int i = 0; i < results.Length; i++)
                        {
                            results[i] = results[i].Cast<object>().ToList();

                            if (!record.NextResult())
                            {
                                for (int j = i + 1; j < results.Length; j++)
                                {
                                    results[j] = null;
                                }

                                break;
                            }
                        }

                        finalResults = new ResultSetCollection(results, null);
                    }
                    finally
                    {
                        record.Dispose();
                    }
                }
                else
                {
                    StreamingScope.Current.Enlist(record);
                    finalResults = new ResultSetCollection(results, record);
                }

            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }

            return finalResults;
        }
    }
}