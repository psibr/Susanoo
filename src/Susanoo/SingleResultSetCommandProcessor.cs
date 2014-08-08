using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Susanoo
{
    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public sealed class SingleResultSetCommandProcessor<TFilter, TResult>
        : ICommandProcessor<TFilter, TResult>, IResultMapper<TResult>, IFluentPipelineFragment
        where TResult : new()
    {
        /// <summary>
        /// The mapping expressions before compilation.
        /// </summary>
        private ICommandResultExpression<TFilter, TResult> _MappingExpressions;

        private ICommandExpression<TFilter> _CommandExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetCommandProcessor{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        public SingleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult> mappings)
        {
            this.MappingExpressions = mappings;
            this._CommandExpression = mappings.CommandExpression;

            this.CompiledMapping = CompileMappings();
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash
        {
            get
            {
                return (this.MappingExpressions.CacheHash * 31) ^ this.CommandExpression.CacheHash;
            }
        }

        /// <summary>
        /// Gets the mapping expressions.
        /// </summary>
        /// <value>The mapping expressions.</value>
        public ICommandResultExpression<TFilter, TResult> MappingExpressions
        {
            get
            {
                return _MappingExpressions;
            }
            private set
            {
                _MappingExpressions = value;
            }
        }

        /// <summary>
        /// Gets the compiled mapping.
        /// </summary>
        /// <value>The compiled mapping.</value>
        public Func<IDataRecord, object> CompiledMapping { get; private set; }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult> results = new List<TResult>();

            ICommandExpression<TFilter> commandExpression = this.MappingExpressions.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results = (((IResultMapper<TResult>)this).MapResult(record, CompiledMapping));
            }

            return results;
        }

        /// <summary>
        /// Maps the result.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> IResultMapper<TResult>.MapResult(IDataReader record, Func<IDataRecord, object> mapping)
        {
            Queue<TResult> queue = new Queue<TResult>();

            while (record.Read())
            {
                queue.Enqueue((TResult)mapping.Invoke(record));
            }

            return queue;
        }

        IEnumerable<TResult> IResultMapper<TResult>.MapResult(IDataReader record)
        {
            return (this as IResultMapper<TResult>).MapResult(record, CompiledMapping);
        }

        /// <summary>
        /// Compiles the result mappings.
        /// </summary>
        /// <returns>Func&lt;IDataRecord, System.Object&gt;.</returns>
        private Func<IDataRecord, object> CompileMappings()
        {
            var mappings = this.MappingExpressions.Export<TResult>();

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
                    Expression.Block(new ParameterExpression[] { localOrdinal },
                        Expression.Assign(localOrdinal,
                            Expression.Call(columnCheckerExp, typeof(ColumnChecker).GetMethod("HasColumn", BindingFlags.Public | BindingFlags.Instance),
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
                                                Expression.New(typeof(ColumnBindingException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }),
                                                    Expression.Constant(pair.Value.PropertyMetadata.Name +
                                                        " encountered an exception on column [" + pair.Value.ActiveAlias + "] when binding"
                                                            + " into property " + pair.Value.PropertyMetadata.Name + " which is CLR type of "
                                                                + pair.Value.PropertyMetadata.PropertyType.Name + "."),
                                                ex
                                                ))))))));
            }

            statements.Add(descriptorExp);

            var body = Expression.Block(new ParameterExpression[] { descriptorExp, columnCheckerExp }, statements);
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
            return await ExecuteAsync(databaseManager, default(TFilter), cancellationToken, explicitParameters).ConfigureAwait(false);
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
            return await ExecuteAsync(databaseManager, default(TFilter), default(CancellationToken), explicitParameters).ConfigureAwait(false);
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
            IEnumerable<TResult> results = new List<TResult>();

            ICommandExpression<TFilter> commandExpression = this.MappingExpressions.CommandExpression;

            using (IDataReader record = await databaseManager
                .ExecuteDataReaderAsync(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    cancellationToken,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)).ConfigureAwait(false))
            {
                results = (((IResultMapper<TResult>)this).MapResult(record, CompiledMapping));
            }

            return results;
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
            return await ExecuteAsync(databaseManager, filter, default(CancellationToken), explicitParameters).ConfigureAwait(false);
        }
    }
}