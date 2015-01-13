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
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public sealed class SingleResultSetCommandProcessor<TFilter, TResult>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult>, IResultMapper<TResult>
        where TResult : new()
    {

        private ColumnChecker _columnChecker;

        private readonly ICommandExpression<TFilter> _commandExpression;

        private readonly MethodInfo _readMethod = typeof (IDataReader).GetMethod("Read",
            BindingFlags.Public | BindingFlags.Instance);

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

            CompiledMapping = typeof (TResult) != typeof (object) ? CompileMappings() : DynamicConversion;

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
            get { return CommandExpression.AllowStoringColumnInfo ? _columnChecker : null; }
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
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
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
            get { return CommandResultExpression.CacheHash; }
        }

        /// <summary>
        /// Gets the mapping expressions.
        /// </summary>
        /// <value>The mapping expressions.</value>
        public ICommandResultExpression<TFilter, TResult> CommandResultExpression { get; private set; }

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
            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;

            IEnumerable<TResult> results = null;

            ICommandExpression<TFilter> commandExpression = CommandResultExpression.CommandExpression;

            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

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
                    results = (((IResultMapper<TResult>) this).MapResult(records, ColumnReport, CompiledMapping));

                    ColumnReport = ((ListResult<TResult>) results).ColumnReport;
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
        /// Dumps all columns into an expando for simple use cases.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="checker">The column checker.</param>
        /// <returns>dynamic.</returns>
        private static IEnumerable<TResult> DynamicConversion(IDataReader reader, ColumnChecker checker)
        {
            var resultSet = new ListResult<TResult>();
            checker = checker ?? new ColumnChecker();

            int fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                IDictionary<string, Object> obj = new ExpandoObject();
                for (var i = 0; i < fieldCount; i++)
                {
                    var name = checker.HasColumn(reader, i);
                    if (name != null)
                        obj.Add(name, reader.GetValue(i));
                }

                resultSet.Add((dynamic) obj);
            }

            resultSet.BuildReport(checker);

            return resultSet;
        }

        /// <summary>
        /// Builds the or regen result mapper from cache.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        /// <returns>IResultMapper&lt;TResult&gt;.</returns>
        public static IResultMapper<TResult> BuildOrRegenResultMapper(
            ICommandResultExpression<TFilter, TResult> commandResultExpression, string name = null)
        {
            return
                (IResultMapper<TResult>)
                    CommandResultExpression<TFilter, TResult>.BuildOrRegenCommandProcessor(commandResultExpression, name);
        }

        /// <summary>
        /// Compiles the result mappings.
        /// </summary>
        /// <returns>Func&lt;IDataRecord, System.Object&gt;.</returns>
        private Func<IDataReader, ColumnChecker, IEnumerable<TResult>> CompileMappings()
        {
            MethodInfo AddLastMethod = typeof (List<TResult>).GetMethod("Add", new[] {typeof (TResult)});

            //Get the properties we care about.
            var mappings = CommandResultExpression.Export<TResult>();

            var outerStatements = new List<Expression>();
            var innerStatements = new List<Expression>();

            ParameterExpression reader = Expression.Parameter(typeof (IDataReader), "reader");
            ParameterExpression columnReport = Expression.Parameter(typeof (ColumnChecker), "columnReport");

            ParameterExpression columnChecker = Expression.Variable(typeof (ColumnChecker), "columnChecker");
            ParameterExpression resultSet = Expression.Variable(typeof (ListResult<TResult>), "resultSet");

            LabelTarget returnStatement = Expression.Label(typeof (IEnumerable<TResult>), "return");

            // resultSet = new LinkedListResult<TResult>();
            outerStatements.Add(Expression.Assign(resultSet, Expression.New(typeof (ListResult<TResult>))));

            // columnChecker = (columnReport == null) ? new ColumnChecker() : columnReport;
            outerStatements.Add(Expression.IfThenElse(Expression.Equal(columnReport, Expression.Constant(null)),
                Expression.Assign(
                    columnChecker, Expression.New(typeof (ColumnChecker))),
                Expression.Assign(columnChecker, columnReport)));

            #region Loop Code

            // var descriptor;
            ParameterExpression descriptor = Expression.Variable(typeof (TResult), "descriptor");

            // if(!reader.Read) { return resultSet; }
            innerStatements.Add(Expression.IfThen(Expression.IsFalse(Expression.Call(reader, _readMethod)),
                Expression.Block(
                    Expression.Call(resultSet,
                        typeof (ListResult<TResult>).GetMethod("BuildReport",
                            BindingFlags.Public | BindingFlags.Instance),
                        columnChecker),
                    Expression.Break(returnStatement, resultSet))));

            // descriptor = new TResult();
            innerStatements.Add(Expression.Assign(
                descriptor, Expression.New(typeof (TResult))));

            foreach (var pair in mappings)
            {
                #region locals

                //These are local variables inside of a block. Similar to defining variables inside an if.

                //var ex;
                var ex = Expression.Variable(typeof (Exception), "ex");
                //var ordinal; 
                var ordinal = Expression.Variable(typeof (int), "ordinal");

                #endregion locals

                innerStatements.Add(
                    Expression.Block(new[] {/* LOCAL */ ordinal},

                        // ordinal = columnChecker.HasColumn(pair.Value.ActiveAlias);
                        Expression.Assign(ordinal,
                            Expression.Call(columnChecker,
                                typeof (ColumnChecker).GetMethod("HasColumn",
                                    new[] {typeof (IDataReader), typeof (string)}),
                                reader,
                                Expression.Constant(pair.Value.ActiveAlias))),

                        // if(ordinal > 0 && !reader.IsDBNull(ordinal)) 
                        Expression.IfThen(
                            Expression.AndAlso(
                                Expression.IsTrue(
                                    Expression.GreaterThanOrEqual(ordinal, Expression.Constant(0))),
                                Expression.IsFalse(
                                    Expression.Call(reader, typeof (IDataRecord).GetMethod("IsDBNull"), ordinal))),
                            // try
                            Expression.TryCatch(
                                Expression.Block(typeof (void),

                                    //Assignment Expression
                                    Expression.Invoke(
                                        pair.Value.AssembleMappingExpression(
                                            Expression.Property(descriptor, pair.Value.PropertyMetadata)),
                                        reader, ordinal)),
                                // catch
                                Expression.Catch(
                                    /* Exception being caught is assigned to ex */ ex,
                                    Expression.Block(typeof (void),

                                        //throw new ColumnBindingException("...", ex); 
                                        Expression.Throw(
                                            Expression.New(ColumnBindingException.MessageAndInnerExceptionConstructorInfo,
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

            //resultSet.AddLast(descriptor);
            innerStatements.Add(Expression.Call(resultSet, AddLastMethod, descriptor));

            var loopBody = Expression.Block(new[] {descriptor}, innerStatements);

            #endregion Loop Code

            outerStatements.Add(Expression.Loop(loopBody, returnStatement));

            var body = Expression.Block(new[] {columnChecker, resultSet}, outerStatements);
            var lambda = Expression.Lambda<Func<IDataReader, ColumnChecker, IEnumerable<TResult>>>(body, reader,
                columnReport);

            var type = CommandManager.DynamicNamespace
                .DefineType(string.Format(CultureInfo.CurrentCulture, "{0}_{1}",
                    typeof (TResult).Name,
                    Guid.NewGuid().ToString().Replace("-", string.Empty)),
                    TypeAttributes.Public);

            lambda.CompileToMethod(type.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static));

            Type dynamicType = type.CreateType();

            return (Func<IDataReader, ColumnChecker, IEnumerable<TResult>>) Delegate
                .CreateDelegate(typeof (Func<IDataReader, ColumnChecker, IEnumerable<TResult>>),
                    dynamicType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static));
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
            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;

            IEnumerable<TResult> results = null;

            ICommandExpression<TFilter> commandExpression = CommandResultExpression.CommandExpression;

            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

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
                    results = (((IResultMapper<TResult>) this).MapResult(records, ColumnReport, CompiledMapping));

                    ColumnReport = ((ListResult<TResult>) results).ColumnReport;
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