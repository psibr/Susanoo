using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo
{
    public class CommandProcessor<TFilter, TResult>
        : ICommandProcessor<TFilter, TResult>
        where TResult : new()
    {
        /// <summary>
        /// The mapping expressions before compilation.
        /// </summary>
        protected CommandResultMappingExpression<TFilter, TResult> mappingExpressions;

        public CommandProcessor(CommandResultMappingExpression<TFilter, TResult> mappings)
        {
            this.mappingExpressions = mappings;

            this.CompiledMapping = CompileMappings();
        }

        protected Func<IDataRecord, object> CompiledMapping { get; private set; }

        /// <summary>
        /// Assembles a data command for an ADO.NET provider, executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public virtual IEnumerable<TResult> Execute(TFilter filter, params IDbDataParameter[] explicitParameters)
        {
            var results = new List<TResult>();

            ICommandExpression<TFilter, TResult> commandExpression = this.mappingExpressions.CommandExpression;

            using (IDataReader record = commandExpression.DatabaseManager
                .ExecuteDataReader(commandExpression.CommandText, commandExpression.DbCommandType, null, commandExpression.BuildParameters(filter, explicitParameters).ToArray()))
            {
                while (record.Read())
                {
                    results.Add((TResult)CompiledMapping.Invoke(record));
                }
            }

            return results;
        }

        /// <summary>
        /// Compiles the mappings.
        /// </summary>
        /// <returns>Func&lt;IDataRecord, System.Object&gt;.</returns>
        protected virtual Func<IDataRecord, object> CompileMappings()
        {
            var mappings = this.mappingExpressions.Export();

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
                                            " encountered an exception on column [" + pair.Value.ReturnName + "] when binding"
                                                + " into property " + pair.Value.PropertyMetadata.Name + " which is CLR type of " + pair.Value.PropertyMetadata.PropertyType.Name + "."),
                                    ex
                                    ))))));
            }

            statements.Add(descriptorExp);

            var body = Expression.Block(new ParameterExpression[] { descriptorExp }, statements);
            var lambda = Expression.Lambda<Func<IDataRecord, object>>(body, readerExp);

            var type = CommandManager.Instance.DynamicNamespace
                .DefineType(string.Format("{0}_{1}", typeof(TResult).Name, Guid.NewGuid().ToString().Replace("-", string.Empty)), TypeAttributes.Public);

            lambda.CompileToMethod(type.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static));

            Type dynamicType = type.CreateType();

            return (Func<IDataRecord, object>)Delegate.CreateDelegate(typeof(Func<IDataRecord, object>), dynamicType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static));
        }
    }
}