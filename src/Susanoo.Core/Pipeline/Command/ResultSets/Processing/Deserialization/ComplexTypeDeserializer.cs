using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Susanoo.Exceptions;

namespace Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization
{
    /// <summary>
    /// Provides compilation and deserialization for complex types.
    /// </summary>
    public static class ComplexTypeDeserializer
    {
        private static readonly MethodInfo ReadMethod = typeof(IDataReader)
            .GetMethod("Read", BindingFlags.Public | BindingFlags.Instance);

        /// <summary>
        /// Compiles mappings.
        /// </summary>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;System.Object&gt;&gt;.</returns>
        public static Func<IDataReader, ColumnChecker, object> Compile(ICommandResultMappingExport mapping, Type resultType)
        {
            var listType = typeof(List<>).MakeGenericType(resultType);
            var listResultType = typeof(ListResult<>).MakeGenericType(resultType);
            var enumerableType = typeof(object);

            MethodInfo addLastMethod = listType.GetMethod("Add", new[] { resultType });

            //Get the properties we care about.
            var mappings = mapping.Export(resultType);

            var outerStatements = new List<Expression>();
            var innerStatements = new List<Expression>();

            ParameterExpression reader = Expression.Parameter(typeof(IDataReader), "reader");
            ParameterExpression columnReport = Expression.Parameter(typeof(ColumnChecker), "columnReport");

            ParameterExpression columnChecker = Expression.Variable(typeof(ColumnChecker), "columnChecker");
            ParameterExpression resultSet = Expression.Variable(listResultType, "resultSet");

            LabelTarget returnStatement = Expression.Label(enumerableType, "return");

            // resultSet = new LinkedListResult<TResult>();
            outerStatements.Add(Expression.Assign(resultSet, Expression.New(listResultType)));

            // columnChecker = (columnReport == null) ? new ColumnChecker() : columnReport;
            outerStatements.Add(Expression.IfThenElse(Expression.Equal(columnReport, Expression.Constant(null)),
                Expression.Assign(
                    columnChecker, Expression.New(typeof(ColumnChecker))),
                Expression.Assign(columnChecker, columnReport)));

            #region Loop Code

            // var descriptor;
            ParameterExpression descriptor = Expression.Variable(resultType, "descriptor");

            // if(!reader.Read) { return resultSet; }
            innerStatements.Add(Expression.IfThen(Expression.IsFalse(Expression.Call(reader, ReadMethod)),
                Expression.Block(
                    Expression.Call(resultSet,
                        listResultType.GetMethod("BuildReport",
                            BindingFlags.Public | BindingFlags.Instance),
                        columnChecker),
                    Expression.Break(returnStatement, resultSet))));

            // descriptor = new TResult();
            innerStatements.Add(Expression.Assign(
                descriptor, Expression.New(resultType)));

            foreach (var pair in mappings)
            {
                #region locals

                //These are local variables inside of a block. Similar to defining variables inside an if.

                //var ex;
                var ex = Expression.Variable(typeof(Exception), "ex");
                //var ordinal; 
                var ordinal = Expression.Variable(typeof(int), "ordinal");

                #endregion locals

                innerStatements.Add(
                    Expression.Block(new[] {/* LOCAL */ ordinal },

                        // ordinal = columnChecker.HasColumn(pair.Value.ActiveAlias);
                        Expression.Assign(ordinal,
                            Expression.Call(columnChecker,
                                typeof(ColumnChecker).GetMethod("HasColumn",
                                    new[] { typeof(IDataReader), typeof(string) }),
                                reader,
                                Expression.Constant(pair.Value.ActiveAlias))),

                        // if(ordinal > 0 && !reader.IsDBNull(ordinal)) 
                        Expression.IfThen(
                            Expression.AndAlso(
                                Expression.IsTrue(
                                    Expression.GreaterThanOrEqual(ordinal, Expression.Constant(0))),
                                Expression.IsFalse(
                                    Expression.Call(reader, typeof(IDataRecord).GetMethod("IsDBNull"), ordinal))),
                    // try
                            Expression.TryCatch(
                                Expression.Block(typeof(void),

                                    //Assignment Expression
                                    Expression.Invoke(
                                        pair.Value.AssembleMappingExpression(
                                            Expression.Property(descriptor, pair.Value.PropertyMetadata)),
                                        reader, ordinal)),
                    // catch
                                Expression.Catch(
                    /* Exception being caught is assigned to ex */ ex,
                                    Expression.Block(typeof(void),

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
            innerStatements.Add(Expression.Call(resultSet, addLastMethod, descriptor));

            var loopBody = Expression.Block(new[] { descriptor }, innerStatements);

            #endregion Loop Code

            outerStatements.Add(Expression.Loop(loopBody, returnStatement));

            var body = Expression.Block(new[] { columnChecker, resultSet }, outerStatements);
            var lambda = Expression.Lambda<Func<IDataReader, ColumnChecker, object>>(body, reader,
                columnReport);

            var type = CommandManager.DynamicNamespace
                .DefineType(string.Format(CultureInfo.CurrentCulture, "{0}_{1}",
                    resultType.Name,
                    Guid.NewGuid().ToString().Replace("-", string.Empty)),
                    TypeAttributes.Public);

            lambda.CompileToMethod(type.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static));

            Type dynamicType = type.CreateType();

            return (Func<IDataReader, ColumnChecker, object>)Delegate
                .CreateDelegate(typeof(Func<IDataReader, ColumnChecker, object>),
                    dynamicType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static));
        }

        /// <summary>
        /// Makes the compiled expression represent the correct type.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public static Func<IDataReader, ColumnChecker, IEnumerable<TResult>> Compile<TResult>(ICommandResultMappingExport mapping, Type resultType)
            where TResult : new()
        {
            var result = Compile(mapping, resultType);

            return (reader, columnMeta) => (IEnumerable<TResult>)(result(reader, columnMeta));
        }
    }
}