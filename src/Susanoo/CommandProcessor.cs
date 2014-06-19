using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo
{
    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class CommandProcessor<TFilter, TResult>
        : ICommandProcessor<TFilter, TResult>
        where TResult : new()
    {
        /// <summary>
        /// The mapping expressions before compilation.
        /// </summary>
        private CommandResultMappingExpression<TFilter, TResult> _mappingExpressions;

        /// <summary>
        /// Gets the mapping expressions.
        /// </summary>
        /// <value>The mapping expressions.</value>
        protected virtual CommandResultMappingExpression<TFilter, TResult> MappingExpressions
        {
            get
            {
                return _mappingExpressions;
            }
            private set
            {
                _mappingExpressions = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessor{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        public CommandProcessor(CommandResultMappingExpression<TFilter, TResult> mappings)
        {
            this.MappingExpressions = mappings;

            this.CompiledMapping = CompileMappings();
        }

        /// <summary>
        /// Gets the compiled mapping.
        /// </summary>
        /// <value>The compiled mapping.</value>
        protected Func<IDataRecord, object> CompiledMapping { get; private set; }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider, executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public virtual IEnumerable<TResult> Execute(TFilter filter, params IDbDataParameter[] explicitParameters)
        {
            var results = new List<TResult>();

            ICommandExpression<TFilter, TResult> commandExpression = this.MappingExpressions.CommandExpression;

            using (IDataReader record = commandExpression.DatabaseManager
                .ExecuteDataReader(commandExpression.CommandText, commandExpression.DBCommandType, null, commandExpression.BuildParameters(filter, explicitParameters).ToArray()))
            {
                while (record.Read())
                {
                    results.Add((TResult)CompiledMapping.Invoke(record));
                }
            }

            return results;
        }

        /// <summary>
        /// Compiles the result mappings.
        /// </summary>
        /// <returns>Func&lt;IDataRecord, System.Object&gt;.</returns>
        protected virtual Func<IDataRecord, object> CompileMappings()
        {
            var mappings = this.MappingExpressions.Export();

            var statements = new List<Expression>();

            ParameterExpression readerExp = Expression.Parameter(typeof(IDataRecord));
            ParameterExpression descriptorExp = Expression.Variable(typeof(TResult), "descriptor");

            statements.Add(Expression.Assign(
                descriptorExp, Expression.New(typeof(TResult))));

            foreach (var pair in mappings)
            {
                var ex = Expression.Variable(typeof(Exception), "ex");

                statements.Add(
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
                                                + " into property " + pair.Value.PropertyMetadata.Name + " which is CLR type of " + pair.Value.PropertyMetadata.PropertyType.Name + "."),
                                    ex
                                    ))))));
            }

            statements.Add(descriptorExp);

            var body = Expression.Block(new ParameterExpression[] { descriptorExp }, statements);
            var lambda = Expression.Lambda<Func<IDataRecord, object>>(body, readerExp);

            var type = CommandManager.Instance.DynamicNamespace
                .DefineType(string.Format(CultureInfo.CurrentCulture, "{0}_{1}", typeof(TResult).Name, Guid.NewGuid().ToString().Replace("-", string.Empty)), TypeAttributes.Public);

            lambda.CompileToMethod(type.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static));

            Type dynamicType = type.CreateType();

            return (Func<IDataRecord, object>)Delegate.CreateDelegate(typeof(Func<IDataRecord, object>), dynamicType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static));
        }
    }
}