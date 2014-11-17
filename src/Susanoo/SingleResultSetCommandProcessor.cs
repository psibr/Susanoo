#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public sealed class SingleResultSetCommandProcessor<TFilter, TResult>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult>, IResultMapper<TResult>
        where TResult : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;

        /// <summary>
        /// Dumps all columns into an expando for simple use cases.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>dynamic.</returns>
        private static dynamic DynamicConversion(IDataRecord record)
        {
            dynamic obj = new ExpandoObject();

            for (var i = 0; i < record.FieldCount; i++)
            {
                ((IDictionary<string, Object>)obj).Add(record.GetName(i), record.GetValue(i));
            }

            return obj;
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        ///  this is confusing and therefor is not allowed.;mode
        /// </exception>
        public ICommandProcessor<TFilter, TResult> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetCommandProcessor{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        public SingleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult> mappings, string name)
            : base(name)
        {
            CommandResultExpression = mappings;
            _commandExpression = mappings.CommandExpression;

            CompiledMapping = typeof(TResult) != typeof(object) ? CompileMappings() : DynamicConversion;

            CommandManager.RegisterCommandProcessor(this, name);
        }

        public static IResultMapper<TResult> BuildOrRegenResultMapper(ICommandResultExpression<TFilter, TResult> commandResultExpression, string name = null)
        {
            return (IResultMapper<TResult>)CommandResultExpression<TFilter, TResult>.BuildOrRegenCommandProcessor(commandResultExpression, name);
        }

        /// <summary>
        ///     Gets the compiled mapping.
        /// </summary>
        /// <value>The compiled mapping.</value>
        private Func<IDataRecord, object> CompiledMapping { get; set; }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get { return CommandResultExpression.CacheHash; }
        }

        /// <summary>
        ///     Gets the mapping expressions.
        /// </summary>
        /// <value>The mapping expressions.</value>
        public ICommandResultExpression<TFilter, TResult> CommandResultExpression { get; private set; }

        /// <summary>
        ///     Assembles a data command for an ADO.NET provider,
        ///     executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        ///     Assembles a data command for an ADO.NET provider,
        ///     executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;

            IEnumerable<TResult> results = null;

            ICommandExpression<TFilter> commandExpression = CommandResultExpression.CommandExpression;

            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                results = value as IEnumerable<TResult>;

                cachedItemPresent = results != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader records = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    results = (((IResultMapper<TResult>)this).MapResult(records, CompiledMapping));
                }

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(results, ResultCachingMode, ResultCachingInterval));
            }

            return results ?? new LinkedList<TResult>();
        }

#if !NETFX40

        /// <summary>
        ///     Assembles a data command for an ADO.NET provider,
        ///     executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
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
        ///     Assembles a data command for an ADO.NET provider,
        ///     executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
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
        ///     Assembles a data command for an ADO.NET provider,
        ///     executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;

            IEnumerable<TResult> results = null;

            ICommandExpression<TFilter> commandExpression = CommandResultExpression.CommandExpression;

            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                results = value as IEnumerable<TResult>;

                cachedItemPresent = results != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader records = await databaseManager
                    .ExecuteDataReaderAsync(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        cancellationToken,
                        parameters)
                    .ConfigureAwait(false))
                {
                    results = (((IResultMapper<TResult>)this).MapResult(records, CompiledMapping));
                }

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(results, ResultCachingMode, ResultCachingInterval));
            }

            return results ?? new LinkedList<TResult>();
        }

        /// <summary>
        ///     Assembles a data command for an ADO.NET provider,
        ///     executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
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

        /// <summary>
        ///     Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> IResultMapper<TResult>.MapResult(IDataReader record, Func<IDataRecord, object> mapping)
        {
            var list = new LinkedList<TResult>();

            while (record.Read())
            {
                list.AddLast((TResult)mapping.Invoke(record));
            }

            return list;
        }

        /// <summary>
        ///     Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> IResultMapper<TResult>.MapResult(IDataReader record)
        {
            return (this as IResultMapper<TResult>).MapResult(record, CompiledMapping);
        }

        /// <summary>
        ///     Compiles the result mappings.
        /// </summary>
        /// <returns>Func&lt;IDataRecord, System.Object&gt;.</returns>
        private Func<IDataRecord, object> CompileMappings()
        {
            var mappings = CommandResultExpression.Export<TResult>();

            var statements = new List<Expression>();

            ParameterExpression readerExp = Expression.Parameter(typeof(IDataRecord));
            ParameterExpression descriptorExp = Expression.Variable(typeof(TResult), "descriptor");
            ParameterExpression columnCheckerExp = Expression.Variable(typeof(ColumnChecker), "columnChecker");

            statements.Add(Expression.Assign(
                columnCheckerExp, Expression.New(typeof(ColumnChecker))));

            statements.Add(Expression.Assign(
                descriptorExp, Expression.New(typeof(TResult))));

            foreach (var pair in mappings)
            {
                var ex = Expression.Variable(typeof(Exception), "ex");

                var localOrdinal = Expression.Variable(typeof(int), "ordinal");

                statements.Add(
                    Expression.Block(new[] { localOrdinal },
                        Expression.Assign(localOrdinal,
                            Expression.Call(columnCheckerExp,
                                typeof(ColumnChecker).GetMethod("HasColumn",
                                    BindingFlags.Public | BindingFlags.Instance),
                                readerExp,
                                Expression.Constant(pair.Value.ActiveAlias))),
                        Expression.IfThen(
                            Expression.AndAlso(
                                Expression.IsTrue(
                                    Expression.GreaterThanOrEqual(localOrdinal, Expression.Constant(0))),
                                Expression.IsFalse(
                                    Expression.Call(readerExp, typeof(IDataRecord).GetMethod("IsDBNull"), localOrdinal))),
                            Expression.TryCatch(
                                Expression.Block(typeof(void),
                                    Expression.Invoke(
                                        pair.Value.AssembleMappingExpression(
                                            Expression.Property(descriptorExp, pair.Value.PropertyMetadata)),
                                        readerExp)),
                                Expression.Catch(
                                    ex,
                                    Expression.Block(typeof(void),
                                        Expression.Throw(
                                            Expression.New(
                                                typeof(ColumnBindingException).GetConstructor(new[] { typeof(string), typeof(Exception) }),
                                                Expression.Constant(pair.Value.PropertyMetadata.Name +
                                                                    " encountered an exception on column [" +
                                                                    pair.Value.ActiveAlias + "] when binding"
                                                                    + " into property " +
                                                                    pair.Value.PropertyMetadata.Name +
                                                                    " which is CLR type of "
                                                                    + pair.Value.PropertyMetadata.PropertyType.Name +
                                                                    "."),
                                                ex
                                                ))))))));
            }

            statements.Add(descriptorExp);

            var body = Expression.Block(new[] { descriptorExp, columnCheckerExp }, statements);
            var lambda = Expression.Lambda<Func<IDataRecord, object>>(body, readerExp);

            var type = CommandManager.DynamicNamespace
                .DefineType(string.Format(CultureInfo.CurrentCulture, "{0}_{1}",
                    typeof(TResult).Name,
                    Guid.NewGuid().ToString().Replace("-", string.Empty)),
                    TypeAttributes.Public);

            lambda.CompileToMethod(type.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static));

            Type dynamicType = type.CreateType();

            return (Func<IDataRecord, object>)Delegate
                .CreateDelegate(typeof(Func<IDataRecord, object>),
                    dynamicType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static));
        }
    }
}