using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Susanoo.Pipeline.Command.ResultSets.Processing
{
    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public sealed class SingleResultSetCommandProcessor<TFilter, TResult> :
        CommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter, TResult>,
        IResultMapper<TResult>
    {
        private ColumnChecker _columnChecker;
        private readonly IDictionary<int, CommandModifier> _commandModifiers = new Dictionary<int, CommandModifier>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetCommandProcessor{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="commandModifiers">The command modifiers.</param>
        /// <param name="name">The name.</param>
        public SingleResultSetCommandProcessor(ICommandResultInfo<TFilter> mappings, IEnumerable<CommandModifier> commandModifiers = null, string name = null)
            : base(mappings)
        {
            CompiledMapping = CommandManager
                .Bootstrapper
                .RetrieveDeserializerResolver()
                .Resolve<TResult>(mappings.GetExporter());

            if(commandModifiers != null)
                foreach (var commandModifier in commandModifiers)
                {
                    _commandModifiers.Add(commandModifier.Priority, commandModifier);
                }

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Gets the compiled mapping.
        /// </summary>
        /// <value>The compiled mapping.</value>
        private Func<IDataReader, ColumnChecker, IEnumerable<TResult>> CompiledMapping { get; set; }

        /// <summary>
        /// Gets or sets the column report.
        /// </summary>
        /// <value>The column report.</value>
        private ColumnChecker ColumnReport
        {
            get { return CommandInfo.AllowStoringColumnInfo ? _columnChecker : null; }
            set
            {
                if (value != null)
                {
                    _columnChecker = value;
                }
            }
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        public ICommandProcessor<TFilter, TResult> EnableResultCaching(CacheMode mode = CacheMode.Permanent,
            double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ClearColumnIndexInfo()
        {
            _columnChecker = new ColumnChecker();
        }

        /// <summary>
        /// Gets the command modifiers.
        /// </summary>
        /// <value>The command modifiers.</value>
        public IOrderedEnumerable<CommandModifier> CommandModifiers
        {
            get
            {
                return _commandModifiers
                    .Select(pair => pair.Value)
                    .OrderBy(modifier => modifier.Priority);
            }
        }

        /// <summary>
        /// Updates the column index information.
        /// </summary>
        /// <param name="info">The column checker.</param>
        public override void UpdateColumnIndexInfo(ColumnChecker info)
        {
            ColumnReport = info;
        }

        /// <summary>
        /// Retrieves a copy of the column index info.
        /// </summary>
        /// <returns>ColumnChecker.</returns>
        public override ColumnChecker RetrieveColumnIndexInfo()
        {
            return ColumnReport.Copy();
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get { return CommandResultInfo.CacheHash; }
        }

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        object IResultMapper.MapResult(IDataReader record)
        {
            return ((IResultMapper<TResult>)this).MapResult(record);
        }



        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;

            IEnumerable<TResult> results = null;

            IExecutableCommandInfo executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandInfo.CommandText,
                DbCommandType = CommandInfo.DbCommandType,
                Parameters = CommandInfo.BuildParameters(databaseManager, filter, explicitParameters)
            };

            executableCommandInfo = CommandModifiers.Aggregate(executableCommandInfo, (info, modifier) => modifier.ModifierFunc(info));

            if (ResultCachingEnabled)
            {
                var parameterAggregate = executableCommandInfo.Parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                cachedItemPresent = TryRetrieveCacheResult(hashCode, out value);

                results = value as IEnumerable<TResult>;

                cachedItemPresent = cachedItemPresent && results != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var records = databaseManager
                        .ExecuteDataReader(
                            executableCommandInfo.CommandText,
                            executableCommandInfo.DbCommandType,
                            executableCommandInfo.Parameters))
                    {
                        results = (((IResultMapper<TResult>)this).MapResult(records, ColumnReport, CompiledMapping));

                        var result = results as ListResult<TResult>;
                        if (result != null)
                            ColumnReport = result.ColumnReport;
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(executableCommandInfo, ex, executableCommandInfo.Parameters);
                }

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(results, ResultCachingMode, ResultCachingInterval));
            }

            return results ?? new LinkedList<TResult>();
        }

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> IResultMapper<TResult>.MapResult(IDataReader reader, ColumnChecker checker,
            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> mapping)
        {
            var result = mapping(reader, checker);
            return result;
        }

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> IResultMapper<TResult>.MapResult(IDataReader record)
        {
            return (this as IResultMapper<TResult>).MapResult(record, ColumnReport, CompiledMapping);
        }

        /// <summary>
        /// Builds the or regen result mapper from cache.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        /// <returns>IResultMapper&lt;TResult&gt;.</returns>
        public static IResultMapper<TResult> BuildOrRegenResultMapper(
            ICommandResultInfo<TFilter> commandResultInfo, string name = null)
        {
            return
                (IResultMapper<TResult>)
                    CommandResultExpression<TFilter, TResult>.BuildOrRegenCommandProcessor(commandResultInfo, name);
        }

#if !NETFX40

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteAsync(databaseManager, default(TFilter), cancellationToken, explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteAsync(databaseManager, default(TFilter), default(CancellationToken), explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;

            IEnumerable<TResult> results = null;

            var parameters = CommandInfo.BuildParameters(databaseManager, filter, explicitParameters);

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                cachedItemPresent = TryRetrieveCacheResult(hashCode, out value);

                results = value as IEnumerable<TResult>;

                cachedItemPresent = cachedItemPresent && results != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var records = await databaseManager
                        .ExecuteDataReaderAsync(
                            CommandInfo.CommandText,
                            CommandInfo.DbCommandType,
                            cancellationToken,
                            parameters)
                        .ConfigureAwait(false))
                    {
                        results = (((IResultMapper<TResult>)this).MapResult(records, ColumnReport, CompiledMapping));

                        ColumnReport = ((ListResult<TResult>)results).ColumnReport;
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(results, ResultCachingMode, ResultCachingInterval));
            }

            return results ?? new LinkedList<TResult>();
        }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteAsync(databaseManager, filter, default(CancellationToken), explicitParameters)
                        .ConfigureAwait(false);
        }

#endif
    }
}