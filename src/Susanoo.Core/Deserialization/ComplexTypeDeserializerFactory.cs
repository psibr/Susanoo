using Susanoo.Exceptions;
using Susanoo.Mapping;
using Susanoo.Processing;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Provides compilation and deserialization for complex types.
    /// </summary>
    public class ComplexTypeDeserializerFactory
        : IDeserializerFactory
    {
        private static readonly ConstructorInfo ColumnCheckerConstructorInfo =
            typeof(ColumnChecker).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                new[] { typeof(int) }, null);

        private static readonly ConcurrentDictionary<string, Func<IDataReader, ColumnChecker, object>>
            DeserializerCache = new ConcurrentDictionary<string, Func<IDataReader, ColumnChecker, object>>();

        /// <summary>
        /// Compiles mappings.
        /// </summary>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;System.Object&gt;&gt;.</returns>
        public static Func<IDataReader, ColumnChecker, object> Compile(string cacheKey, IMappingExport mapping, Type resultType)
        {
            //Get the properties we care about.
            var mappings = mapping.Export();

            var statements = new List<Expression>();

            var reader = Expression.Parameter(typeof(IDataReader), "reader");
            var columnReport = Expression.Parameter(typeof(ColumnChecker), "columnReport");

            var columnChecker = Expression.Variable(typeof(ColumnChecker), "columnChecker");

            // columnChecker = (columnReport == null) ? new ColumnChecker() : columnReport;
            statements.Add(Expression.IfThenElse(Expression.Equal(columnReport, Expression.Constant(null)),
                Expression.Assign(
                    columnChecker, Expression.New(ColumnCheckerConstructorInfo,
                    Expression.Convert(Expression.Property(Expression.Convert(reader, typeof(IDataRecord)), "FieldCount"), typeof(int?)))),
                Expression.Assign(columnChecker, columnReport)));

            // var descriptor;
            var descriptor = Expression.Variable(resultType, "descriptor");

            // descriptor = new TResult();
            statements.Add(Expression.Assign(
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

                statements.Add(
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

            statements.Add(descriptor);

            var body = Expression.Block(new[] { columnChecker, descriptor }, statements);
            var lambda = Expression.Lambda<Func<IDataReader, ColumnChecker, object>>(body, reader,
                columnReport);

            var type = CommandManager.DynamicNamespace
                .DefineType(string.Format(CultureInfo.CurrentCulture, "{0}_{1}",
                    resultType.Name, cacheKey));

            lambda.CompileToMethod(type.DefineMethod("MapResult", MethodAttributes.Public | MethodAttributes.Static));

            var dynamicType = type.CreateType();

            return (Func<IDataReader, ColumnChecker, object>)Delegate
                .CreateDelegate(typeof(Func<IDataReader, ColumnChecker, object>),
                    dynamicType.GetMethod("MapResult", BindingFlags.Public | BindingFlags.Static));
        }

        /// <summary>
        /// Makes the compiled expression represent the correct type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>Func&lt;IDataReader, ColumnChecker, IEnumerable&lt;TResult&gt;&gt;.</returns>
        public static Func<IDataReader, ColumnChecker, IEnumerable<TResult>> CastResults<TResult>(
            Func<IDataReader, ColumnChecker, IEnumerable> compiledFunc)
        {
            var result = compiledFunc;

            Func<IDataReader, ColumnChecker, IEnumerable<TResult>> typedResult =
                (reader, columnMeta) => (IEnumerable<TResult>)(result(reader, columnMeta));

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
            var cacheKey = GetCacheKey(typeof(TResult), mappings).ToString();

            Func<IDataReader, ColumnChecker, object> deserializer;


            if (!DeserializerCache.TryGetValue(cacheKey, out deserializer))
            {
                deserializer = Compile(cacheKey, mappings, typeof(TResult));
                DeserializerCache.TryAdd(cacheKey, deserializer);
            }

            return new ComplexTypeDeserializer<TResult>(mappings, deserializer);
        }

        /// <summary>
        /// Builds a deserializer.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IDeserializer BuildDeserializer(Type resultType, IMappingExport mappings)
        {
            var cacheKey = GetCacheKey(resultType, mappings).ToString();

            Func<IDataReader, ColumnChecker, object> deserializer;

            if (!DeserializerCache.TryGetValue(cacheKey, out deserializer))
            {
                deserializer = Compile(cacheKey, mappings, resultType);
                DeserializerCache.TryAdd(cacheKey, deserializer);
            }

            return new ComplexTypeDeserializer(mappings, resultType, deserializer);
        }

        private static BigInteger GetCacheKey(Type resultType, IMappingExport mappings)
        {
            if (mappings is DefaultResultMapping)
                return HashBuilder.Compute(resultType.AssemblyQualifiedName);

            return HashBuilder.Compute(mappings.Export()
                .Aggregate(resultType.AssemblyQualifiedName, (seed, kvp)
                    => seed + kvp.Key + kvp.Value.ActiveAlias + kvp.Value.PropertyMetadata.Name));
        }
    }
}