using Susanoo.Command;
using Susanoo.Deserialization;
using Susanoo.ResultSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Susanoo.Exceptions;

namespace Susanoo.Processing
{
    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public sealed class SingleResultSetCommandProcessor<TFilter, TResult> :
        SingleResultSetCommandProcessorStructure<TFilter, TResult>,
        ISingleResultSetCommandProcessor<TFilter, TResult>
    {
        private ColumnChecker _columnChecker = new ColumnChecker();

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetCommandProcessor{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="deserializerResolver">The deserializer resolver.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        public SingleResultSetCommandProcessor(
            IDeserializerResolver deserializerResolver,
            ICommandResultInfo<TFilter> mappings,
            string name = null)
        {
            CommandResultInfo = mappings;

            CompiledMapping = deserializerResolver
                .ResolveDeserializer<TResult>(mappings);
        }

        /// <summary>
        /// Gets the compiled mapping.
        /// </summary>
        /// <value>The compiled mapping.</value>
        private IDeserializer<TResult> CompiledMapping { get; }

        /// <summary>
        /// Gets or sets the column report.
        /// </summary>
        /// <value>The column report.</value>
        private ColumnChecker ColumnReport 
            => CommandBuilderInfo.AllowStoringColumnInfo ? _columnChecker : null;

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        public override void ClearColumnIndexInfo()
        {
            _columnChecker = new ColumnChecker();
        }

        /// <summary>
        /// Retrieves a copy of the column index info.
        /// </summary>
        /// <returns>ColumnChecker.</returns>
        public override ColumnChecker RetrieveColumnIndexInfo()
        {
            return ColumnReport;
        }

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override ISingleResultSetCommandProcessor<TFilter, TResult> InterceptOrProxyWith(Func<ISingleResultSetCommandProcessor<TFilter, TResult>, ISingleResultSetCommandProcessor<TFilter, TResult>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public override IEnumerable<TResult> Execute(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            try
            {
                using (var records = databaseManager
                    .ExecuteDataReader(
                        executableCommandInfo.CommandText,
                        executableCommandInfo.DbCommandType,
                        Timeout,
                        executableCommandInfo.Parameters))

                {
                    var results = CompiledMapping.Deserialize(records, ColumnReport);

                    return results ?? Enumerable.Empty<TResult>();
                }
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }
        }

#if !NETFX40

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public override async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            try
            {
                using (var records = await databaseManager
                    .ExecuteDataReaderAsync(
                        executableCommandInfo.CommandText,
                        executableCommandInfo.DbCommandType,
                        Timeout,
                        cancellationToken,
                        executableCommandInfo.Parameters)
                    .ConfigureAwait(false))
                {
                    var results = CompiledMapping.Deserialize(records, ColumnReport);

                    return results ?? Enumerable.Empty<TResult>();
                }
            }
            catch (Exception ex)
            {
                throw new SusanooExecutionException(ex, executableCommandInfo, Timeout, executableCommandInfo.Parameters);
            }
        }
#endif
    }
}