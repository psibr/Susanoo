using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Susanoo.Exceptions;
using Susanoo.Mapping;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Provides compilation and deserialization for complex types.
    /// </summary>
    public class ComplexTypeDeserializerFactory
        : IDeserializerFactory
    {
        private static readonly MethodInfo ReadMethod = typeof(IDataReader)
            .GetMethod("Read", BindingFlags.Public | BindingFlags.Instance);

        private static readonly ConstructorInfo ColumnCheckerConstructorInfo =
            typeof (ColumnChecker).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                new[] {typeof (int)}, null);

        private static readonly ConcurrentDictionary<BigInteger, Func<IDataReader, ColumnChecker, IEnumerable>> 
            DeserilializerCache = new ConcurrentDictionary<BigInteger, Func<IDataReader, ColumnChecker, IEnumerable>>();

        /// <summary>
        /// Compiles mappings.
        /// </summary>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;System.Object&gt;&gt;.</returns>
        public static Func<IDataReader, ColumnChecker, IEnumerable> Compile(IMappingExport mapping, Type resultType)
        {
            var listType = typeof(List<>).MakeGenericType(resultType);
            var listResultType = typeof(List<>).MakeGenericType(resultType);
            var enumerableType = typeof(IEnumerable);

            var addLastMethod = listType.GetMethod("Add", new[] { resultType });

            //Get the properties we care about.
            var mappings = mapping.Export();

            var outerStatements = new List<Expression>();
            var innerStatements = new List<Expression>();

            var reader = Expression.Parameter(typeof(IDataReader), "reader");
            var columnReport = Expression.Parameter(typeof(ColumnChecker), "columnReport");

            var columnChecker = Expression.Variable(typeof(ColumnChecker), "columnChecker");
            var resultSet = Expression.Variable(listResultType, "resultSet");

            var returnStatement = Expression.Label(enumerableType, "return");

            // resultSet = new LinkedListResult<TResult>();
            outerStatements.Add(Expression.Assign(resultSet, Expression.New(listResultType)));

            // columnChecker = (columnReport == null) ? new ColumnChecker() : columnReport;
            outerStatements.Add(Expression.IfThenElse(Expression.Equal(columnReport, Expression.Constant(null)),
                Expression.Assign(
                    columnChecker, Expression.New(ColumnCheckerConstructorInfo, 
                    Expression.Convert(Expression.Property(Expression.Convert(reader, typeof(IDataRecord)), "FieldCount"), typeof(int?)))),
                Expression.Assign(columnChecker, columnReport)));

            #region Loop Code

            // var descriptor;
            var descriptor = Expression.Variable(resultType, "descriptor");

            // if(!reader.Read) { return resultSet; }
            innerStatements.Add(Expression.IfThen(Expression.IsFalse(Expression.Call(reader, ReadMethod)),
                Expression.Block(Expression.Break(returnStatement, resultSet))));

            // descriptor = new TResult();
            innerStatements.Add(Expression.Assign(
                descriptor, Expression.New(resultType)));

            // ReSharper disable once LoopCanBeConvertedToQuery
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
            var lambda = Expression.Lambda<Func<IDataReader, ColumnChecker, IEnumerable>>(body, reader,
                columnReport);

            var type = CommandManager.DynamicNamespace
                .DefineType(string.Format(CultureInfo.CurrentCulture, "{0}_{1}",
                    resultType.Name,
                    Guid.NewGuid().ToString().Replace("-", string.Empty)),
                    TypeAttributes.Public);

            lambda.CompileToMethod(type.DefineMethod("Map", MethodAttributes.Public | MethodAttributes.Static));

            var dynamicType = type.CreateType();

            return (Func<IDataReader, ColumnChecker, IEnumerable>)Delegate
                .CreateDelegate(typeof(Func<IDataReader, ColumnChecker, IEnumerable>),
                    dynamicType.GetMethod("Map", BindingFlags.Public | BindingFlags.Static));
        }

        /// <summary>
        /// Makes the compiled expression represent the correct type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public static Func<IDataReader, ColumnChecker, IEnumerable<TResult>> CastResults<TResult>(Func<IDataReader, ColumnChecker, IEnumerable> compiledFunc)
        {
            var result = compiledFunc;

            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> typedResult =
                (reader, columnMeta) => (IEnumerable<TResult>) (result(reader, columnMeta));

            return typedResult;
        }

        /// <summary>
        /// Determines whether this deserializer applies to the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this instance can deserialize; otherwise, <c>false</c>.</returns>
        public bool CanDeserialize(Type type)
        {
            return true;
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IDeserializer<TResult> BuildDeserializer<TResult>(IMappingExport mappings)
        {
            var cacheKey = GetCacheKey(typeof(TResult), mappings);

            Func<IDataReader, ColumnChecker, IEnumerable> deserializer;

            if (!DeserilializerCache.TryGetValue(cacheKey, out deserializer))
            {

                deserializer = Compile(mappings, typeof(TResult));
                DeserilializerCache.TryAdd(cacheKey, deserializer);
            }

            return new Deserializer<TResult>(CastResults<TResult>(deserializer));
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IDeserializer BuildDeserializer(Type resultType, IMappingExport mappings)
        {
            var cacheKey = GetCacheKey(resultType, mappings);

            Func<IDataReader, ColumnChecker, IEnumerable> deserializer;

            if (!DeserilializerCache.TryGetValue(cacheKey, out deserializer))
            {
                
                deserializer = Compile(mappings, resultType);
                DeserilializerCache.TryAdd(cacheKey, deserializer);
            }


            return new Deserializer(resultType, deserializer);
        }

        private BigInteger GetCacheKey(Type resultType, IMappingExport mappings)
        {
            if (mappings is DefaultResultMapping)
                return HashBuilder.Compute(resultType.AssemblyQualifiedName);

            return HashBuilder.Compute(mappings.Export()
                .Aggregate(resultType.AssemblyQualifiedName, (seed, kvp)
                    => seed + kvp.Key + kvp.Value.ActiveAlias + kvp.Value.PropertyMetadata.Name));
        }
    }
}